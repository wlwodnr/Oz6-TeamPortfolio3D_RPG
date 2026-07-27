using UnityEngine;

public class ItemDropEntity : MonoBehaviour, IGameObjectEntity
{
    [SerializeField] private SpriteRenderer Renderer_ItemIcon;

    private int _instanceId = -1;
    private string _itemDataId;
    private int _count;
    private bool _isPickupRequested;
    private Camera _mainCamera;

    public int InstanceId
    {
        get { return _instanceId; }
    }

    public string ItemDataId
    {
        get { return _itemDataId; }
    }

    public int Count
    {
        get { return _count; }
    }

    private void OnEnable()
    {
        _isPickupRequested = false;
        _mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (Renderer_ItemIcon == null || Renderer_ItemIcon.sprite == null)
        {
            return;
        }

        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }

        if (_mainCamera != null)
        {
            Renderer_ItemIcon.transform.rotation = _mainCamera.transform.rotation;
        }
    }

    public void InitEntity(int instanceId, string dataId)
    {
        _instanceId = instanceId;
        _itemDataId = dataId;
        _count = 1;
        _isPickupRequested = false;

        RefreshVisual();
    }

    public void SetDropCount(int count)
    {
        _count = Mathf.Max(1, count);
    }

    public void ResetEntity()
    {
        _instanceId = -1;
        _itemDataId = string.Empty;
        _count = 0;
        _isPickupRequested = false;

        if (Renderer_ItemIcon != null)
        {
            Renderer_ItemIcon.sprite = null;
        }
    }

    private void RefreshVisual()
    {
        if (Renderer_ItemIcon == null)
        {
            Debug.LogWarning($"[{gameObject.name}] Renderer_ItemIcon이 연결되지 않았습니다.");
            return;
        }

        Sprite itemIcon = ItemDataBase.GetItemIcon(_itemDataId);

        if (itemIcon == null)
        {
            Debug.LogWarning($"[{gameObject.name}] 드랍 아이템 아이콘을 찾을 수 없습니다. ItemDataId: {_itemDataId}");
            return;
        }

        Renderer_ItemIcon.sprite = itemIcon;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isPickupRequested == true)
        {
            return;
        }

        PlayerEntity playerEntity = other.GetComponentInParent<PlayerEntity>();

        if (playerEntity == null)
        {
            return;
        }

        if (NetworkManager.Inst == null || NetworkManager.Inst.InventoryService == null)
        {
            Debug.LogWarning($"[{gameObject.name}] InventoryService가 없어 아이템을 획득할 수 없습니다. ItemDataId: {_itemDataId}, Count: {_count}");
            return;
        }

        bool isAdded = NetworkManager.Inst.InventoryService.RequestAddItem(_itemDataId, _count);

        if (isAdded == false)
        {
            Debug.LogWarning($"[{gameObject.name}] 인벤토리에 아이템을 추가하지 못했습니다. ItemDataId: {_itemDataId}, Count: {_count}");
            return;
        }

        if (GameObjectManager.Instance == null)
        {
            Debug.LogWarning($"[{gameObject.name}] GameObjectManager가 없어 드랍 아이템을 반환할 수 없습니다. InstanceId: {_instanceId}");
            return;
        }

        _isPickupRequested = true;
        GameObjectManager.Instance.RequestDisableGameObject(_instanceId);
    }
}
