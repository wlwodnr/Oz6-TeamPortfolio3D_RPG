using UnityEngine;

public class PlayerEntity : MonoBehaviour, IGameObjectEntity
{
    [SerializeField] private int _instanceId = -1;
    [SerializeField] private string _playerDataId;

    public int InstanceId
    {
        get 
        { 
            return _instanceId; 
        }
    }

    public string PlayerDataId
    {
        get 
        { 
            return _playerDataId; 
        }
    }

    public void InitEntity(int instanceId, string dataId)
    {
        _instanceId = instanceId;
        _playerDataId = dataId;

        gameObject.SetActive(true);
    }

    public void ResetEntity()
    {
        _instanceId = -1;
        _playerDataId = string.Empty;
    }
}