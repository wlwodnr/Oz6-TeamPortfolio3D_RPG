using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class Treasure : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _treasureChild;
    [SerializeField] private GameObject _coinEffect;
    private CancellationTokenSource _cts;
    public float rotationSpeed = 90f; 
    private float targetX = 120f;

    [Header("아이템 드랍 설정")]
    public string[] dropItemIds;         
    public int[] dropCounts;           
    public Transform dropSpawnPoint;     
    public float popForceMin = 3f;       
    public float popForceMax = 6f;
    public bool CanInteract { set; get; }

    private void Awake()
    {
        CanInteract = true;
    }

    public void Interact()
    {
        CanInteract = false;
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        RotateRoutineAsync(targetX, _cts.Token).Forget();
        SpawnAndDropItems();
    }

    private async UniTaskVoid RotateRoutineAsync(float targetX, CancellationToken cancellationToken)
    {
        Quaternion startRotation = _treasureChild.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(targetX, _treasureChild.transform.eulerAngles.y, _treasureChild.transform.eulerAngles.z);

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
        if (dropItemIds == null || dropItemIds.Length == 0) return;

        for (int i = 0; i < dropItemIds.Length; i++)
        {
            string itemId = dropItemIds[i];
            int count = (dropCounts != null && i < dropCounts.Length) ? dropCounts[i] : 1;

            if (string.IsNullOrEmpty(itemId)) continue;

            var spawnPoint = dropSpawnPoint.position;
            spawnPoint.y += 1;

            int instanceId = GameObjectManager.Instance.RequestSpawnTreasureItemDrop(spawnPoint, itemId, count);

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
