using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private NavMeshAgent Agent_NavMesh;
    [SerializeField] private EnemyStatus Status_Enemy;
    [SerializeField] private LayerMask Layer_Target;

    [SerializeField] private EnemyEntity Entity_Enemy;
    [SerializeField] private Animator Animator_Enemy;

    [Header("아이템 드랍 설정")]
    [SerializeField, Min(0f)] private float _itemDropHeightOffset = 0.5f;

    private string _monsterDataId;
    private MonsterData _monsterData;
    private Transform _currentTarget;

    private SpawnSpot _spawnOriginSpot;

    private bool _isDisableRequested = false;

    private EnemyAIState _currentStateEnum;
    private IEnemyAIState _currentState;
    private Dictionary<EnemyAIState, IEnemyAIState> _states;


    private int InstanceId
    {
        get
        {
            if (Entity_Enemy == null)
            {
                Debug.LogWarning($"[{gameObject.name}] Entity_Enemy가 없어 InstanceId를 가져올 수 없습니다.");
                return -1;
            }

            return Entity_Enemy.InstanceId;
        }
    }

    public Transform CurrnetTarget { get { return _currentTarget; } }
    public MonsterData MonsterData { get { return _monsterData; } }

    public EnemyStatus Status { get { return Status_Enemy; }  }


    public Vector3 SpawnPosition
    {
        get
        {
            if (_spawnOriginSpot == null)
            {
                Debug.LogWarning($"[{gameObject.name}] 할당된 SpawnSpot이 없어 현재 위치를 리턴합니다.");
                return transform.position;
            }
            return _spawnOriginSpot.transform.position;
        }
    }

    private void OnEnable()
    {
        
        _isDisableRequested = false;

        if (Status_Enemy == null)
        {
            Debug.LogWarning($"[{gameObject.name}] Status_Enemy가 없어 사망 이벤트를 등록할 수 없습니다.");
            return;
        }

        Status_Enemy.OnDeadEvent -= OnEnemyDead;
        Status_Enemy.OnDeadEvent += OnEnemyDead;
    }

    private void OnDisable()
    {
        if (Status_Enemy == null)
        {
            return;
        }
        Status_Enemy.OnDeadEvent -= OnEnemyDead;
    }

    private void Awake()
    {
        if (Agent_NavMesh == null)
        {
            Debug.LogWarning($"[{gameObject.name}] Agent_NavMesh가 인스펙터에 연결되지 않았습니다.");
        }

        if (Status_Enemy == null)
        {
            Debug.LogWarning($"[{gameObject.name}] Status_Enemy가 인스펙터에 연결되지 않았습니다.");
        }

        if (Entity_Enemy == null)
        {
            Debug.LogWarning($"[{gameObject.name}] Entity_Enemy가 인스펙터에 연결되지 않았습니다.");
        }

        _states = new Dictionary<EnemyAIState, IEnemyAIState>
        {
            {EnemyAIState.Idle, new EnemyAIState_Idle() },
            {EnemyAIState.Attack, new EnemyAIState_Attack() },
            {EnemyAIState.Dead, new EnemyAIState_Dead() },
            {EnemyAIState.Walk, new EnemyAIState_Walk() },
            {EnemyAIState.RangeAttack, new EnemyAIState_RangeAttack() },
            {EnemyAIState.SpecialAttack, new EnemyAIState_SpecialAttack() }
        };
    }

    private void Update()
    {
        if (Status_Enemy != null && Status_Enemy.IsDead) return;

        _currentState?.UpdateState(this);
    }

    public void InitEnemyInfo(int generatedId, string monsterDataId, SpawnSpot ownerSpot)
    {
        _monsterDataId = monsterDataId;
        _spawnOriginSpot = ownerSpot;
        _currentTarget = null;
        _isDisableRequested = false;

        int entityInstanceId = InstanceId;
       
        //확인용
        if (InstanceId != generatedId)
        {
            Debug.LogWarning($"[{gameObject.name}] generatedId와 EnemyEntity.InstanceId가 다릅니다. generatedId: {generatedId}, EntityId: {InstanceId}");
        }

        _monsterData = GameDataManager.Instance.GetMonsterData(_monsterDataId);
        if (_monsterData == null)
        {
            Debug.LogWarning($"MonsterData를 찾을 수 없습니다. MonsterDataId: {_monsterDataId}");
            return;
        }

        if (Status_Enemy != null)
        {
            Status_Enemy.InitStatus(_monsterData);
        }

        InitNavMeshAgent();

    }

    private void InitNavMeshAgent()
    {
        if (Agent_NavMesh == null)
        {
            Debug.LogWarning($"[{gameObject.name}] Agent_NavMesh가 연결되지 않아 NavMeshAgent를 초기화할 수 없습니다.");
            return;
        }

        if (_monsterData == null)
        {
            Debug.LogWarning($"[{gameObject.name}] MonsterData가 없어 NavMeshAgent를 초기화할 수 없습니다.");
            return;
        }

        Agent_NavMesh.speed = _monsterData.MoveSpeed;

        if (Agent_NavMesh.enabled == false)
        {
            return;
        }

        if (Agent_NavMesh.isOnNavMesh == false)
        {
            Debug.LogWarning($"[{gameObject.name}] NavMesh 위에 있지 않아 이동 정지를 해제할 수 없습니다.");
            return;
        }

        Agent_NavMesh.isStopped = false;
        Agent_NavMesh.ResetPath();
    }

    private void OnEnemyDead()
    {
        if (_isDisableRequested == true)
        {
            return;
        }

        _isDisableRequested = true;

        StopMoving();
        ClearTarget();

        if(Agent_NavMesh != null)
        {
            Agent_NavMesh.enabled = false;
        }

        Debug.Log($"[{gameObject.name}] AI 작동 중지");

        NotifyKillQuestProgress();
        RequestExperienceReward();
        RequestGoldReward();
        RequestItemDrops();

        RequestDisableSelf();
    }

    private void RequestExperienceReward()
    {
        if (_monsterData == null || _monsterData.DropEXP <= 0f)
        {
            return;
        }

        if (NetworkManager.Inst == null || NetworkManager.Inst.LocalPlayerService == null)
        {
            Debug.LogWarning($"[{gameObject.name}] LocalPlayerService가 없어 경험치를 지급할 수 없습니다. MonsterDataId: {_monsterDataId}, Experience: {_monsterData.DropEXP}");
            return;
        }

        NetworkManager.Inst.LocalPlayerService.RequestGiveExpToLocalPlayer(_monsterData.DropEXP);
    }

    private void RequestGoldReward()
    {
        if (_monsterData == null || _monsterData.DropGold <= 0)
        {
            return;
        }

        if (NetworkManager.Inst == null || NetworkManager.Inst.LocalPlayerService == null)
        {
            Debug.LogWarning($"[{gameObject.name}] LocalPlayerService가 없어 골드를 지급할 수 없습니다. MonsterDataId: {_monsterDataId}, Gold: {_monsterData.DropGold}");
            return;
        }

        NetworkManager.Inst.LocalPlayerService.RequestGiveGoldToLocalPlayer(_monsterData.DropGold);
    }

    private void RequestItemDrops()
    {
        if (GameDataManager.Instance == null)
        {
            Debug.LogWarning($"[{gameObject.name}] GameDataManager가 없어 드랍 데이터를 조회할 수 없습니다.");
            return;
        }

        if (GameObjectManager.Instance == null)
        {
            Debug.LogWarning($"[{gameObject.name}] GameObjectManager가 없어 아이템을 드랍할 수 없습니다.");
            return;
        }

        string monsterDataId = GetEnemyDataId();

        if (string.IsNullOrEmpty(monsterDataId))
        {
            Debug.LogWarning($"[{gameObject.name}] MonsterDataId가 없어 아이템을 드랍할 수 없습니다.");
            return;
        }

        if (DropItem.TryCreate(GameDataManager.Instance.DropDataList, monsterDataId, out DropItem dropItem) == false) return;

        Vector3 itemDropSpawnPosition = transform.position + Vector3.up * _itemDropHeightOffset;
        int itemDropInstanceId = GameObjectManager.Instance.RequestSpawnItemDrop(itemDropSpawnPosition, dropItem);

        if (itemDropInstanceId < 0)
        {
            Debug.LogWarning($"[{gameObject.name}] 아이템 드랍 생성에 실패했습니다. ItemDataId: {dropItem.ItemDataId}, Count: {dropItem.Count}");
        }
    }

    private void NotifyKillQuestProgress()
    {
        if(QuestManager.Instance == null)
        {
            Debug.LogWarning($"[{gameObject.name}] QuestManager가 없어 몬스터 처치 진행도를 전달할 수 없습니다.");
            return;
        }

        string enemyDataId = GetEnemyDataId();

        if(string.IsNullOrEmpty(enemyDataId) )
        {
            Debug.LogWarning($"[{gameObject.name}] EnemyDataId가 없어 몬스터 처치 진행도를 전달할 수 없습니다.");

            return;
        }
        QuestManager.Instance.NotifyEnemyKilled(enemyDataId);
    }

    private string GetEnemyDataId()
    {
        if(Entity_Enemy != null && string.IsNullOrEmpty(Entity_Enemy.EnemyDataId) == false)
        {
            return Entity_Enemy.EnemyDataId;
        }

        return _monsterDataId;
    }


    private void RequestDisableSelf()
    {
        if (GameObjectManager.Instance == null)
        {
            Debug.LogWarning($"[{gameObject.name}] GameObjectManager가 없어 비활성화 요청을 할 수 없습니다.");
            return;
        }

        int instanceId = InstanceId;

        if (instanceId < 0)
        {
            Debug.LogWarning($"[{gameObject.name}] 유효하지 않은 InstanceId입니다. InstanceId: {instanceId}");
            return;
        }

        GameObjectManager.Instance.RequestDisableGameObject(instanceId);
    }
    public void ResetEnemyAIForPool(SpawnSpot newSpawnSpot)
    {
        _spawnOriginSpot = newSpawnSpot;
        _currentTarget = null;
        _isDisableRequested = false;

        if (Status_Enemy != null)
        {
            Status_Enemy.ResetStatus();
        }

        if(Agent_NavMesh != null)
        {
            Agent_NavMesh.enabled = true;
            if (_monsterData != null)
            {
                Agent_NavMesh.speed = _monsterData.MoveSpeed;
            }
            if (Agent_NavMesh.isOnNavMesh)
            {
                Agent_NavMesh.ResetPath();
                Agent_NavMesh.isStopped = false;
            }
        }

        ResetAnimatorForPool();
        ResetAIStateForPool();
    }

    public void PrepareEnemyAIForPool()
    {
        ClearTarget();

        if (Agent_NavMesh != null)
        {
            if (Agent_NavMesh.enabled == true && Agent_NavMesh.isOnNavMesh == true)
            {
                Agent_NavMesh.isStopped = true;
                Agent_NavMesh.ResetPath();
            }

            Agent_NavMesh.enabled = false;
        }

        if (Status_Enemy != null)
        {
            Status_Enemy.PrepareStatusForPool();
        }

        ResetAnimatorForPool();
        _currentState = null;
        _currentStateEnum = EnemyAIState.Idle;
        _spawnOriginSpot = null;
        _monsterDataId = string.Empty;
        _monsterData = null;
        _isDisableRequested = false;
    }

    private void ResetAnimatorForPool()
    {
        if (Animator_Enemy == null)
        {
            return;
        }

        Animator_Enemy.ResetTrigger("IsAttack");
        Animator_Enemy.Rebind();
        Animator_Enemy.Update(0f);
    }

    private void ResetAIStateForPool()
    {
        if (_states == null || _states.ContainsKey(EnemyAIState.Idle) == false)
        {
            return;
        }

        _currentStateEnum = EnemyAIState.Idle;
        _currentState = _states[EnemyAIState.Idle];
        _currentState.EnterState(this);
    }

    public Animator GetEntityAnimator()
    {
        return Animator_Enemy;
    }

    public void ChangeState(EnemyAIState newState)
    {
        if (_currentStateEnum == newState && newState != EnemyAIState.Attack && newState != EnemyAIState.RangeAttack && newState != EnemyAIState.SpecialAttack)
        {
            return;
        }

        if (_states.ContainsKey(newState) == false) { return; }
        
        if(!IsStateChangeable(newState))
        {
            Debug.LogWarning($"[{gameObject.name}] {newState} 상태로 전환 실패 (IsStateChangeable 차단)");
            return;
        }

        if (_currentState != null)
        {
            _currentState.ExitState(this);
        }

        _currentState = _states[newState];

        Debug.Log($"<color=yellow>[FSM 상태 변경]</color> {gameObject.name} : {_currentStateEnum} -> {newState}");

        _currentState.EnterState(this);
        _currentStateEnum = newState;
    }

    public bool IsStateChangeable(EnemyAIState newState)
    {
        if(newState == EnemyAIState.Attack)
        {
            if(_currentStateEnum == EnemyAIState.Walk)
            {
                return false;
            }
        }

        return true;
    }


    
    public void RequestAttack()
    {
        if(Status_Enemy.IsDead) return;

        Status_Enemy.AttackPlayer();

        StopMoving();
        Debug.Log($"{gameObject.name}이(가) 타겟에게 공격을 시도합니다! (데미지: {_monsterData.BaseAttack})");
    }

    public void StopMoving()
    {
        if(Agent_NavMesh != null && Agent_NavMesh.isOnNavMesh)
        {
            Agent_NavMesh.isStopped = true;
            Agent_NavMesh.ResetPath();
        }
    }

    public void ClearTarget()
    {
        _currentTarget = null;
    }


   


}
