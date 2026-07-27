using UnityEngine;

public class PlayerEntity : MonoBehaviour, IGameObjectEntity
{
    [SerializeField] private int _instanceId = -1;
    [SerializeField] private string _playerDataId;

    private PlayerController _playerController;

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

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    public void InitEntity(int instanceId, string dataId)
    {
        _instanceId = instanceId;
        _playerDataId = dataId;
    }

    public void ResetEntity()
    {
        _instanceId = -1;
        _playerDataId = string.Empty;

        if (_playerController != null)
        {
            _playerController.ResetControllerForPool();
        }
    }
}
