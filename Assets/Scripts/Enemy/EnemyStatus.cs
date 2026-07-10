using System;
using UnityEngine;

public class EnemyStatus : MonoBehaviour, IDamageable
{
    //빌드용 임시 체력
    [SerializeField] private int _temporaryMaxHp = 30;

    private int _currentHp;
    private bool _isDead;
    private MonsterData _monsterData;

    public bool IsDead {  get { return _isDead; } }
    public int CurrentHp { get { return _currentHp; } }

    public event Action OnDeadEvent;

    private void OnEnable()
    {
        ResetStatus();
    }

    public void InitStatus(MonsterData monsterData)
    {
        _monsterData = monsterData;

        ResetStatus();

    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (_isDead) return;

        if (damageInfo == null)
        {
            Debug.LogWarning($"[{gameObject.name}] DamageInfo가 null이어서 데미지를 처리할 수 없습니다.");
            return;
        }

        //TO Do: GameUTil 만들기 혹은 연동 기다리기
        //int finalDamage = GameUtil.CalcCharacterFinalDamage();

        //_currentHP -= finalDamage;
        //Debug.Log($"[{gameObject.name}] 피격당함! 최종 데미지: {finalDamage}, 남은 HP: {_currentHp} (공격자: { damageInfo.Attacker.name})");

        //TO DO:피격 애니메이션 및 이펙트 처리 요청
        //
        if (damageInfo.BaseDamage <= 0)
        {
            Debug.LogWarning(
                $"[{gameObject.name}] 유효하지 않은 데미지입니다. " +
                $"Damage: {damageInfo.BaseDamage}"
            );

            return;
        }

        int appliedDamage = damageInfo.BaseDamage;
        _currentHp = Mathf.Max(0, _currentHp - appliedDamage);

        if (_currentHp <= 0)
        {
            SetDead();
        }

    }

    private void SetDead()
    {
        //중복 로직 방지
        if (_isDead == true)
        {
            return;
        }

        _isDead = true;
        _currentHp = 0;
        Debug.Log($"[{gameObject.name}] HP가 0 이하가 되어 사망 처리되었습니다.");

        OnDeadEvent?.Invoke();
    }

    public void ResetStatus()
    {
        _isDead = false;
        if(_monsterData != null)
        {
            _currentHp = _monsterData.BaseHp;

        }
        else
        {
            //임시 최대 체력 사용
            _currentHp = Mathf.Max(_temporaryMaxHp);
        }
        //
        Debug.Log($"[{gameObject.name}] 상태가 초기화되었습니다.");
    }

#if UNITY_EDITOR
    // PlayerAttack이 완성되기 전 테스트용
    [ContextMenu("TEST/10 데미지 받기")]
    private void TestTakeDamage()
    {
        DamageInfo testDamageInfo = new DamageInfo(
            10,
            false,
            transform.position,
            Vector3.zero,
            gameObject
        );

        TakeDamage(testDamageInfo);
    }
#endif
}



