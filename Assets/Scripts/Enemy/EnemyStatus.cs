using System;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EnemyStatus : MonoBehaviour, IDamageable
{
    //빌드용 임시 체력
    [SerializeField] private int _temporaryMaxHp = 30;

    [SerializeField] private LayerMask LayerMask_Player;


    private int _currentHp;
    private bool _isDead;
    private MonsterData _monsterData;
    private MonsterModel _monsterModel;

    private int _enemyAttack;
    private float _enemyMoveSpeed;
    private float _detectRange;
    private float _attackRange;
    private float _stopDistance;

    public bool IsDead {  get { return _isDead; } }
    public int CurrentHp { get { return _currentHp; } }
    public int MaxHp { get { return _monsterData != null ? _monsterData.BaseHp : _temporaryMaxHp; } }

    public int BaseAttack { get { return _enemyAttack; } }
    public float MoveSpeed { get { return _enemyMoveSpeed; } }
    public float DetectRange { get { return _detectRange; } }
    public float AttackRange { get { return _attackRange; } }
    public float StopDistance { get { return _stopDistance; } }

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

    public void ReinforceAttack()
    {
        _enemyAttack = _enemyAttack + 20;
    }

    public void AttackPlayer()
    {
        if (GameObjectManager.Instance == null)
        {
            Debug.LogWarning("GameObjectManager.Instance가 존재하지 않습니다.");
            return;
        }

        int playerInstanceId = GameObjectManager.Instance.PlayerInstanceId;
        GameObject playerObject = GameObjectManager.Instance.GetGameObjectCanBeNull(playerInstanceId);

        Vector3 knockbackDir = Vector3.zero;
        knockbackDir = transform.forward;
        knockbackDir.y = 0f;

        IGameObjectEntity targetEntity = playerObject.GetComponentInParent<IGameObjectEntity>();

        float finalAtkDamage = _enemyAttack;
        int finalCalculatedDamage = Mathf.RoundToInt(finalAtkDamage);

        DamageInfo dmgInfo = new(
                finalCalculatedDamage,
                false,
                playerObject.transform.position,
                knockbackDir,
                gameObject
            );

        GameObjectManager.Instance.RequestTakeDamage(targetEntity.InstanceId, dmgInfo);


    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (_isDead) return;

        if (damageInfo == null)
        {
            Debug.LogWarning($"[{gameObject.name}] DamageInfo가 null이어서 데미지를 처리할 수 없습니다.");
            return;
        }

        int appliedDamage = damageInfo.BaseDamage;
        if (damageInfo.IsCritical)
        {
            appliedDamage = appliedDamage * 2;
        }


        if (appliedDamage <= 0)
        {
            Debug.LogWarning(
                $"[{gameObject.name}] 유효하지 않은 데미지입니다. " +
                $"Damage: {appliedDamage}"
            );

            return;
        }

        _currentHp = Mathf.Max(0, _currentHp - appliedDamage);


        Debug.Log($"[{name}] TakeDamage() 실행 확인");
        transform.GetComponent<Rigidbody>().AddForce(damageInfo.KnockbackDir * 10f, ForceMode.Impulse);




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
            _attackRange = _monsterData.AttackRange;
            _detectRange = _monsterData.DetectRange;
            _enemyAttack = _monsterData.BaseAttack;
            _enemyMoveSpeed = _monsterData.MoveSpeed;
            _stopDistance = _monsterData.StopDistance;
        }
        else
        {
            //임시 최대 체력 사용
            _currentHp = Mathf.Max(_temporaryMaxHp);
        }
        //
        Debug.Log($"[{gameObject.name}] 상태가 초기화되었습니다.");
    }

    public void PrepareStatusForPool()
    {
        _monsterData = null;
        _currentHp = 0;
        _isDead = false;
        _enemyAttack = 0;
        _enemyMoveSpeed = 0f;
        _detectRange = 0f;
        _attackRange = 0f;
        _stopDistance = 0f;
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



