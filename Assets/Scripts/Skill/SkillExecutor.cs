using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class SkillExecutor : MonoBehaviour
{
    [SerializeField] private Animator Animator_Owner;
    [SerializeField] private Rigidbody Rigidbody_Owner;
    [SerializeField] private LayerMask LayerMask_Enemy;

    private PlayerModel _playerModel;
    private SkillTracker _skillTracker;
    private CancellationTokenSource _cts;

    public void Init(PlayerModel playerModel, SkillTracker tracker)
    {
        _playerModel = playerModel;
        _skillTracker = tracker;
        _cts = new CancellationTokenSource();
    }

    private void OnDisable()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }

    public void TryExecuteSkill(string skillId)
    {
        if (GameDataManager.Instance == null || _skillTracker == null || _playerModel == null) return;

        ActiveSkillData skillData = GameDataManager.Instance.GetActiveSkillData(skillId);

        if (skillData == null)
        {
            Debug.LogWarning($"{skillData.Name} 스킬의 데이터를 불러올 수 없습니다.");
            return;
        }

        if (!_skillTracker.SkillModel.IsSkillReady(skillId)) return;
        if (!_playerModel.HasLearnedActive(skillId)) return;

        if (_playerModel.Info.CurMp < skillData.Cost)
        {
            Debug.LogWarning($"{skillData.Name} 스킬을 사용하기 위한 마나가 부족합니다.");
            return;
        }

        ExecuteSkillTask(skillData).Forget();
    }

    private async UniTaskVoid ExecuteSkillTask(ActiveSkillData data)
    {
        _skillTracker.SkillModel.StartCoolTime(data.Id, data.CoolDown);

        _playerModel.Info.CurMp -= data.Cost;

        if (data.DamageMultiplier <= 0f && (data.TargetMode == "Hunt" || data.TargetMode == "Boss"))
        {
            CharacterMode targetMode = _skillTracker.SkillModel.CurrentMode == CharacterMode.Hunt ? CharacterMode.Boss : CharacterMode.Hunt;
            _skillTracker.ChangeMode(targetMode);
            return;
        }

        if (Animator_Owner != null)
        {
            Animator_Owner.SetTrigger(data.Id);
        }

        if (data.CastTime > 0f)
        {
            bool isCanceled = await UniTask.Delay(TimeSpan.FromSeconds(data.CastTime), cancellationToken: _cts.Token).SuppressCancellationThrow();
            if (isCanceled) return;
        }

        if (data.DashForce > 0f)
        {
            if (Rigidbody_Owner != null)
            {
                Vector3 moveDir = transform.forward;
                Vector3 calculatedVelocity = moveDir * data.DashForce;

                Rigidbody_Owner.linearVelocity = new Vector3(Rigidbody_Owner.linearVelocity.x, 0f, Rigidbody_Owner.linearVelocity.z);
                Rigidbody_Owner.AddForce(calculatedVelocity, ForceMode.Impulse);
            }
            else
            {
                Debug.LogWarning($"{data.Id} 스킬로 {gameObject.name}을(를) 공격했으나 Rigidbody_Owner가 할당되지 않았습니다.");
            }
        }

        if (data.MultiHitPercentList != null && data.MultiHitPercentList.Count() > 0)
        {
            for (int i = 0; i < data.MultiHitPercentList.Count; i++)
            {
                if (_playerModel == null || gameObject == null) return;

                ProcessHitDetection(data, data.MultiHitPercentList[i]);

                if (i < data.MultiHitPercentList.Count - 1)
                {
                    float interval = data.HitInterval > 0f ? data.HitInterval : 0.1f;
                    bool isCanceled = await UniTask.Delay(TimeSpan.FromSeconds(interval), cancellationToken: _cts.Token).SuppressCancellationThrow();
                    if (isCanceled) return;
                }
            }
        }
        else if (data.DamageMultiplier > 0f)
        {
            ProcessHitDetection(data, 1.0f);
        }
    }

    private void ProcessHitDetection(ActiveSkillData data, float hitPercent)
    {
        if (GameObjectManager.Instance == null)
        {
            Debug.LogWarning("GameObjectManager.Instance가 존재하지 않습니다.");
            return;
        }

        float height = data.AttackHeight > 0f ? data.AttackHeight : 1.2f;

        Vector3 center = transform.position + transform.forward * (data.AttackRange / 2f) + Vector3.up;
        var halfExtents = new Vector3(data.AttackRange / 2f, height, data.AttackRange / 2f);
        Collider[] hitEnemies = Physics.OverlapBox(center, halfExtents, transform.rotation, LayerMask_Enemy);

        HashSet<int> attackedInstanceIdSet = new();
        int currentHitCount = 0;

        foreach (Collider enemy in hitEnemies)
        {
            if (enemy == null) continue;
            if (currentHitCount >= data.TargetCount) break;

            IGameObjectEntity targetEntity = enemy.GetComponentInParent<IGameObjectEntity>();
            if (targetEntity == null || targetEntity.InstanceId < 0)
            {
                Debug.LogWarning($"적 오브젝트 [{enemy.gameObject.name}]에서 유효한 IGameObjectEntity를 획득하지 못했습니다.");
                continue;
            }

            if (!attackedInstanceIdSet.Add(targetEntity.InstanceId)) continue;

            float finalAtkDamage = _playerModel.GetStatValue(StatType.AttackPower);
            int finalCalculatedDamage = Mathf.RoundToInt(finalAtkDamage * data.DamageMultiplier * hitPercent);

            Vector3 knockbackDir = Vector3.zero;
            if (string.Equals(data.CrowdControl, "KnockBack", StringComparison.OrdinalIgnoreCase))
            {
                knockbackDir = transform.forward;
                knockbackDir.y = 0f;
            }

            DamageInfo dmgInfo = new(
                finalCalculatedDamage,
                false,
                enemy.transform.position,
                knockbackDir,
                gameObject
            );

            GameObjectManager.Instance.RequestTakeDamage(targetEntity.InstanceId, dmgInfo);
            currentHitCount++;
        }
    }
}