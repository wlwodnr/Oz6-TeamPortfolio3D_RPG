using UnityEngine;

public class EnemyEntity : MonoBehaviour, IGameObjectEntity
{
    [SerializeField] private int _instanceId = -1;

    [SerializeField] private string _enemyDataId;

    private EnemyAI _enemyAI;
    private Rigidbody _rigidbody;

    public int InstanceId
    {
        get { return _instanceId; }
    }


    public string EnemyDataId
    {
        get { return _enemyDataId; }
    }

    private void Awake()
    {
        _enemyAI = GetComponent<EnemyAI>();
        if (_enemyAI == null)
        {
            _enemyAI = GetComponentInChildren<EnemyAI>(true);
        }

        _rigidbody = GetComponent<Rigidbody>();
    }

    public void InitEntity(int instanceId, string dataId)
    {
        _instanceId = instanceId;
        _enemyDataId = dataId;

        Debug.Log($"Enemy 초기화 완료. InstanceId: {_instanceId}, EnemyDataId: {_enemyDataId}");
    }

    public void ResetEntity()
    {
        Debug.Log($"Enemy 리셋. InstanceId: {_instanceId}");

        _instanceId = -1;
        _enemyDataId = string.Empty;

        if (_enemyAI != null)
        {
            _enemyAI.PrepareEnemyAIForPool();
        }

        if (_rigidbody != null)
        {
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
    }


}
