using UnityEngine;

public class EnemyEntity : MonoBehaviour, IGameObjectEntity
{
    [SerializeField] private int _instanceId = -1;

    [SerializeField] private string _enemyDataId;

    public int InstanceId
    {
        get { return _instanceId; }
    }


    public string EnemyDataId
    {
        get { return _enemyDataId; }
    }

    public void InitEntity(int instanceId, string dataId)
    {
        _instanceId = instanceId;
        _enemyDataId = dataId;

        gameObject.SetActive(true);

        //JU ToDo 데이터 드리븐으로 EnemyData를 갖고오는 작업을 해야한다.
        Debug.Log($"Enemy 초기화 완료. InstanceId: {_instanceId}, EnemyDataId: {_enemyDataId}");
    }

    public void ResetEntity()
    {
        Debug.Log($"Enemy 리셋. InstanceId: {_instanceId}");

        _instanceId = -1;
        _enemyDataId = string.Empty;

        //JU ToDo 나중에 Stat 등이 완성되면 HP, 상태, 타겟, 에니메이션, NavMeshAgent 등을 초기화 해야함
    }


}
