using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private NavMeshAgent Agent_NavMesh;
    [SerializeField] private EnemyStatus Status_Enemy;
    [SerializeField] private LayerMask Layer_Target;

    //1번 EnemyEntity 라는 컴포넌트에서 Enemy의 InstanceId 값을 저장중. 이 값을 갖고오도록 우선적으로 시킴.
    [SerializeField] private EnemyEntity Entity_Enemy;

    private int _instanceId;
    private string _monsterDataId;
    private MonsterData _monsterData;
    private Transform _currentTarget;

    private SpawnSpot _spawnOriginSpot;

    private bool _isDisableRequested = false;

    //1번 
    private int IntanceId
    {
        get
        {
            if(Entity_Enemy != null)
            {
                return Entity_Enemy.InstanceId;
            }
            return _instanceId;
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
        //Status_Enemy 스크립트 null체크
        _isDisableRequested = false;

        if (Status_Enemy == null)
        {
            Debug.LogWarning($"[{gameObject.name}] Status_Enemy가 없어 사망 이벤트를 등록할 수 없습니다.");
            return;
        }

        //혹시나 있을 이미 들어가있는 경우를 대비하여 빼고 넣기
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
    }


    public void InitEnemyInfo(int generatedId, string monsterDataId, SpawnSpot ownerSpot)
    {
        _instanceId = generatedId;
        _monsterDataId = monsterDataId;
        _spawnOriginSpot = ownerSpot;

        //_monsterData = GameDataManager.Instance.GetMonsterData(_monsterDataId);
        if(_monsterData == null)
        {
            Debug.LogWarning($"MonsterData를 찾을 수 없습니다. MonsterDataId: {_monsterDataId}");
            return;
        }

        //null인지 한번만 더 체크 null 체크는 들어갈 수 있는 한 많이
        if (Status_Enemy != null)
        {
            Status_Enemy.InitStatus(_monsterData);
        }

        if (Agent_NavMesh != null)
        {
            Agent_NavMesh.speed = _monsterData.MoveSpeed;

            //죽었을 때 비활성화 하기에, 활성화 되어있는지 한번 더 체크
            if (Agent_NavMesh == null)
            {
                Debug.LogWarning($"[{gameObject.name}] Agent_NavMesh가 연결되지 않아 이동 속도를 설정할 수 없습니다.");
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
        }

    }

    private void OnEnemyDead()
    {
        StopMoving();
        ClearTarget();

        if(Agent_NavMesh != null)
        {
            Agent_NavMesh.enabled = false;
        }

        Debug.Log($"[{gameObject.name}] AI 작동 중지");

        GameObjectManager.Instance.RequestDisableGameObject(_instanceId);
    }

    public void ResetEnemyAIForPool(SpawnSpot newSpawnSpot)
    {
        _spawnOriginSpot = newSpawnSpot;
        _currentTarget = null;


        Status_Enemy.ResetStatus();

        if(Agent_NavMesh != null)
        {
            Agent_NavMesh.enabled = true;
            if (Agent_NavMesh.isOnNavMesh)
            {
                Agent_NavMesh.isStopped = false;
            }
        }

        
    }

    // BT에서 호출할 메서드들

    public void MoveToPosition(Vector3 targetPosition)
    {
        if (Status_Enemy.IsDead || Agent_NavMesh == null || !Agent_NavMesh.gameObject.activeInHierarchy) return;

        if (Agent_NavMesh.isOnNavMesh)
        {
            Agent_NavMesh.isStopped = false;
            Agent_NavMesh.SetDestination(targetPosition);
        }
    }

    public void ChaseTarget()
    {
        if(_currentTarget != null)
        {
            MoveToPosition(_currentTarget.position);
        }
    }

    public void RequestAttack()
    {
        if(Status_Enemy.IsDead) return;

        StopMoving();
        Debug.Log($"{gameObject.name}이(가) 타겟에게 공격을 시도합니다! (데미지: {_monsterData.BaseAttack})");
    }

    public void StopMoving()
    {
        if(Agent_NavMesh != null && Agent_NavMesh.isOnNavMesh)
        {
            Agent_NavMesh.isStopped = true;
        }
    }

    public void ClearTarget()
    {
        _currentTarget = null;
    }

    public bool SearchTarget()
    {
        if(Status_Enemy.IsDead || _monsterData == null) return false;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _monsterData.DetectRange, Layer_Target);

        if(hitColliders.Length > 0)
        {
            _currentTarget = hitColliders[0].transform;
            return true;
        }

        return false;
    }

    public bool CheckAttackRange()
    {
        if (_currentTarget == null || _monsterData == null) return false;

        float distance = Vector3.Distance(transform.position, _currentTarget.position);
        return distance <= _monsterData.AttackRange;
    }

    public bool CheckExceededSpawnLimit()
    {
        if (_monsterData == null) return false;

        float distanceFromHome = Vector3.Distance(transform.position, SpawnPosition);
        return distanceFromHome > _monsterData.SpawnLimitRange;
    }



}
