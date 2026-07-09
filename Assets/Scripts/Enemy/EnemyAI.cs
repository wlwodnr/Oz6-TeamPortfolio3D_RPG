using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private NavMeshAgent Agent_NavMesh;
    [SerializeField] private EnemyStatus Status_Enemy;
    [SerializeField] private LayerMask Layer_Target;

    private int _instanceId;
    private string _monsterDataId;
    private MonsterData _monsterData;
    private Transform _currentTarget;

    private SpawnSpot _spawnOriginSpot;

    public int IntanceId { get { return _instanceId; }  }
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
        Status_Enemy.OnDeadEvent += OnEnemyDead;
    }

    private void OnDisable()
    {
        Status_Enemy.OnDeadEvent -= OnEnemyDead;
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

        Status_Enemy.InitStatus(_monsterData);

        if(Agent_NavMesh != null)
        {
            Agent_NavMesh.speed = _monsterData.MoveSpeed;
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
