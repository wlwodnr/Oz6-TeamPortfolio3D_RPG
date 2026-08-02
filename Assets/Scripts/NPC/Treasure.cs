using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour, IInteractable, IGameObjectEntity
{
    [SerializeField] private GameObject _treasureChild;
    [SerializeField] private GameObject _coinEffect;
    [SerializeField] private int _instanceId = -1;
    private CancellationTokenSource _cts;
    private Quaternion _closedLocalRotation;
    public float rotationSpeed = 90f; 
    private float targetX = 120f;

    [Header("아이템 드랍 설정")]
    public List<string> dropItemIds = new List<string>();     
    public Transform dropSpawnPoint;     
    public float popForceMin = 3f;       
    public float popForceMax = 6f;
    public bool CanInteract { set; get; }

    [Header("상자 데이터")]
    public string treasureId;
    public bool isOpened;
    public int dropCoins;

    public int InstanceId
    {
        get
        {
            return _instanceId;
        }
    }

    private void Awake()
    {
        if (_treasureChild != null)
        {
            _closedLocalRotation = _treasureChild.transform.localRotation;
        }
        else
        {
            Debug.LogWarning($"Treasure: [{gameObject.name}] Treasure Child가 등록되지 않았습니다.", this);
        }

        CanInteract = true;  // GameObjectManager를 거치지않은 테스트 소환용
    }

    private void OnDisable()
    {
        CancelRotateTask();
    }

    public void InitEntity(int instanceId, string dataId)
    {
        _instanceId = instanceId;

        if (_instanceId <= 0)
        {
            CanInteract = false;
            Debug.LogWarning($"Treasure: [{gameObject.name}] 유효하지 않은 InstanceId입니다. InstanceId: {_instanceId}", this);
            return;
        }
        if (string.IsNullOrWhiteSpace(dataId))
        {
            CanInteract = false;
            Debug.LogWarning($"Treasure: [{gameObject.name}] TreasureDataId가 비어 있습니다. InstanceId: {_instanceId}", this);
            return;
        }
        if (ItemDataBase.TreasureDataDic.TryGetValue(dataId, out TreasureDropData treasureData) == false)
        {
            CanInteract = false;
            Debug.LogWarning($"Treasure: [{gameObject.name}] TreasureDropData를 찾을 수 없습니다. InstanceId: {_instanceId}, TreasureDataId: {dataId}", this);
            return;
        }
        if (treasureData.ItemDropCount > 0 && ItemDataBase.ItemDataDic.Count == 0)
        {
            CanInteract = false;
            Debug.LogWarning($"Treasure: [{gameObject.name}] ItemData가 로드되지 않아 드롭 목록을 생성할 수 없습니다. InstanceId: {_instanceId}, TreasureDataId: {dataId}", this);
            return;
        }

        InitData(treasureData);

        Debug.Log($"Treasure 초기화 완료. InstanceId: {_instanceId}, TreasureDataId: {treasureId}", this);
    }

    public void ResetEntity()
    {
        Debug.Log($"Treasure 리셋. InstanceId: {_instanceId}, TreasureDataId: {treasureId}", this);

        CancelRotateTask();

        _instanceId = -1;
        treasureId = string.Empty;
        isOpened = false;
        dropCoins = 0;
        CanInteract = false;
        dropItemIds.Clear();

        ResetTreasureVisualToClosed();
    }

    public void InitData(TreasureDropData data)
    {
        if (data == null)
        {
            CanInteract = false;
            Debug.LogWarning($"Treasure: [{gameObject.name}] TreasureDropData가 null입니다. InstanceId: {_instanceId}", this);
            return;
        }

        CancelRotateTask();
        dropItemIds.Clear();
        ResetTreasureVisualToClosed();

        treasureId = data.TreasureId;
        dropCoins = data.DropCoins;

        for(int i = 0; i < data.ItemDropCount; i++)
        {
            var item = ItemDataBase.GetRandomItem();
            if(item != null)
            {
                dropItemIds.Add(item.ItemId);
            }
        }

        if (GameObjectManager.Instance == null)
        {
            CanInteract = false;
            Debug.LogWarning($"Treasure: [{gameObject.name}] GameObjectManager가 없어 열린 상자 상태를 확인할 수 없습니다. TreasureDataId: {treasureId}", this);
            return;
        }

        if(GameObjectManager.Instance.HasOpenedTreasureId(treasureId) == true)
        {
            isOpened = true;
            CanInteract = false;
            Quaternion targetRotation = Quaternion.Euler(targetX, 0, 0);
            _treasureChild.transform.rotation = targetRotation;
        }
        else
        {
            isOpened = false;
            CanInteract = true;
        }
    }

    public void Interact()
    {
        CanInteract = false;
        isOpened = true;
        GameObjectManager.Instance.AddOpenedTreasureId(treasureId);
        CancelRotateTask();
        _cts = new CancellationTokenSource();

        RotateRoutineAsync(targetX, _cts.Token).Forget();
        SpawnAndDropItems();
    }

    private async UniTask RotateRoutineAsync(float targetX, CancellationToken cancellationToken)
    {
        Quaternion startRotation = _treasureChild.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(targetX, 0, 0);

        float duration = 0.5f; 
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (cancellationToken.IsCancellationRequested) return;

            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            _treasureChild.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }

        _treasureChild.transform.rotation = targetRotation;
    }

    private void SpawnAndDropItems()
    {
        if (dropItemIds == null || dropItemIds.Count == 0) return;

        for (int i = 0; i < dropItemIds.Count; i++)
        {
            string itemId = dropItemIds[i];

            if (string.IsNullOrEmpty(itemId)) continue;

            var spawnPoint = dropSpawnPoint.position;
            spawnPoint.y += 1;

            int instanceId = GameObjectManager.Instance.RequestSpawnTreasureItemDrop(spawnPoint, itemId, 1);

            if (instanceId < 0) continue;

            GameObject itemObject = GameObjectManager.Instance.GetGameObjectCanBeNull(instanceId);
            if (itemObject != null)
            {
                Rigidbody rb = itemObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)).normalized;
                    float randomForce = Random.Range(popForceMin, popForceMax);

                    rb.AddForce(randomDir * randomForce, ForceMode.Impulse);
                }
            }
        }
        var spawnPos = dropSpawnPoint.position;
        spawnPos.y += 1f;

        GameObject vfx = Instantiate(_coinEffect, spawnPos, _coinEffect.transform.rotation);

        Destroy(vfx, 1.5f);

    }

    private void OnDestroy()
    {
        CancelRotateTask();
    }

    private void CancelRotateTask()
    {
        if (_cts == null)
        {
            return;
        }

        _cts.Cancel();
        _cts.Dispose();
        _cts = null;
    }

    private void ResetTreasureVisualToClosed()
    {
        if (_treasureChild == null)
        {
            return;
        }

        _treasureChild.transform.localRotation = _closedLocalRotation;
    }
}
