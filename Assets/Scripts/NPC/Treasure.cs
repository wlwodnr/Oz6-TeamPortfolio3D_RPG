using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _treasureChild;
    [SerializeField] private GameObject _coinEffect;
    private CancellationTokenSource _cts;
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

    private void Awake()
    {
        CanInteract = true;  // GameObjectManager를 거치지않은 테스트 소환용
    }

    public void InitData(TreasureDropData data)
    {
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
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        RotateRoutineAsync(targetX, _cts.Token).Forget();
        SpawnAndDropItems();
    }

    private async UniTaskVoid RotateRoutineAsync(float targetX, CancellationToken cancellationToken)
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
        _cts?.Cancel();
        _cts?.Dispose();
    }
}
