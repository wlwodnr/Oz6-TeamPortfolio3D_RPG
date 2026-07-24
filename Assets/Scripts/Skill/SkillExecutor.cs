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
    [SerializeField] private AnimatorOverrideController overrideController;

    [Header("CurrentSkillState")]
    private ActiveSkillData _currentSkillData;
    private int _currentHitIndex = 0;
    private bool _isExecutingSkill = false;

    private PlayerModel _playerModel;
    private SkillTracker _skillTracker;
    private CancellationTokenSource _cts;

    public void Init(PlayerModel playerModel, SkillTracker tracker)
    {
        _playerModel = playerModel;
        _skillTracker = tracker;
        _cts = new CancellationTokenSource();
    }

    public void TestInit()
    {
        _cts = new CancellationTokenSource();
    }
    private void Awake()
    {
        if (Animator_Owner == null)
            Animator_Owner = GetComponent<Animator>();

        if (Rigidbody_Owner == null)
            Rigidbody_Owner = GetComponent<Rigidbody>();


        TestInit();
    }
    private void OnDisable()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }

    public void TryExecuteSkill(string skillId)
    {
        if (_skillTracker == null && SkillTracker.Instance != null)
        {
            _skillTracker = SkillTracker.Instance;
        }

        if (_playerModel == null)
        {
            _playerModel = new PlayerModel();
        }

        if (GameDataManager.Instance == null || _skillTracker == null)
        {
            Debug.LogError($"GameDataManager 또는 SkillTracker가 존재하지 않습니다.");
            return;
        }

        ActiveSkillData skillData = GameDataManager.Instance.GetActiveSkillData(skillId);
        if (skillData == null) return;

        if (!_skillTracker.SkillModel.IsSkillReady(skillId)) return;

        if (!skillData.IsModeChange && !_playerModel.HasLearnedActive(skillId))
        {
            Debug.LogWarning($"{skillData.Name} 스킬이 습득되지 않은 상태입니다.");
            return;
        }

        ExecuteSkillTask(skillData).Forget();
    }

    private async UniTaskVoid ExecuteSkillTask(ActiveSkillData data)
    {
        _skillTracker.SkillModel.StartCoolTime(data.Id, data.CoolDown);

        if (_playerModel != null && _playerModel.Info != null)
        {
            _playerModel.Info.CurMp -= data.Cost;
        }

        if (data.IsModeChange)
        {
            CharacterMode currentMode = _skillTracker.SkillModel.CurrentMode;

            CharacterMode targetMode = (currentMode == CharacterMode.Hunt) ? CharacterMode.Boss : CharacterMode.Hunt;

            Debug.Log($"모드 변경. {currentMode} -> {targetMode}");

            _skillTracker.ChangeMode(targetMode);

            return;
        }

        //애니메이터 관련 신규 로직
        _currentSkillData = data;
        _currentHitIndex = 0;
        _isExecutingSkill = true;

        if (overrideController != null && data.SkillClip != null)
        {
            overrideController["Skill_Cast"] = data.SkillClip;
        }

        if (Animator_Owner != null)
        {
            Animator_Owner.SetTrigger("SkillTrigger");
        }
        //123

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

            Debug.Log($"{enemy.gameObject.name}");

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
            float knockbackForce = 0f;

            if (string.Equals(data.CrowdControl, "KnockBack", StringComparison.OrdinalIgnoreCase))
            {
                knockbackDir = transform.forward;
                knockbackDir.y = 0f;
                knockbackForce = data.KnockBackForce;
            }

            DamageInfo dmgInfo = new(
                finalCalculatedDamage,
                false,
                enemy.transform.position,
                knockbackDir,
                knockbackForce,
                gameObject
            );

            GameObjectManager.Instance.RequestTakeDamage(targetEntity.InstanceId, dmgInfo);
            currentHitCount++;
        }

    }

    /// 애니메이션 관련 신규 로직
    public void AttackHandler()
    {
        if (!_isExecutingSkill || _currentSkillData == null)
        {
            Debug.LogWarning("실행 중인 스킬 데이터가 없거나 이미 종료된 스킬입니다.");
            return;
        }

        float hitPercent = 1f;

        if (_currentSkillData.MultiHitPercentList != null &&
            _currentHitIndex < _currentSkillData.MultiHitPercentList.Count)
        {
            hitPercent = _currentSkillData.MultiHitPercentList[_currentHitIndex];
        }

        Debug.Log($"[{_currentSkillData.Name}] 스킬 {_currentHitIndex + 1}타 발동. (타격 비율: {hitPercent})");

        ProcessHitDetection(_currentSkillData, hitPercent);

        _currentHitIndex++;
    }

    public void AttackEnd()
    {
        if (_currentSkillData != null)
        {
            Debug.Log($"[{_currentSkillData.Name}] 스킬 사용 종료. 스킬 정보를 초기화합니다.");
        }

        _currentSkillData = null;
        _currentHitIndex = 0;
        _isExecutingSkill = false;
    }
}