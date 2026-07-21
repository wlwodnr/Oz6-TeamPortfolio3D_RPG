using Cysharp.Threading.Tasks;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class DamageTextUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TextMesh_Damage;
    [SerializeField] private float _floatSpeed = 1.0f;
    [SerializeField] private float _duration = 0.8f;

    private Color _originalColor;
    private CancellationTokenSource _cts;

    private void Awake()
    {
        if (TextMesh_Damage != null)
        {
            _originalColor = TextMesh_Damage.color;
        }
    }

    public void Init(float damageAmount, Vector3 worldPosition)
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();

        transform.position = Camera.main.WorldToScreenPoint(worldPosition);

        if (TextMesh_Damage != null)
        {
            TextMesh_Damage.text = Mathf.CeilToInt(damageAmount).ToString();
            TextMesh_Damage.color = _originalColor;
        }

        gameObject.SetActive(true);

        AnimateAndReleaseAsync(_cts.Token).Forget();
    }

    private async UniTaskVoid AnimateAndReleaseAsync(CancellationToken cancellationToken)
    {
        float elapsedTime = 0f;
        Vector3 startPos = transform.position;

        var combinedToken = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken,
            this.GetCancellationTokenOnDestroy()
        ).Token;

        while (elapsedTime < _duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / _duration;

            // 위로 떠오르는 연출
            transform.position = startPos + Vector3.up * (_floatSpeed * elapsedTime * 50f);

            // 알파값 페이드아웃 연출
            if (TextMesh_Damage != null)
            {
                Color color = _originalColor;
                color.a = Mathf.Lerp(1f, 0f, progress);
                TextMesh_Damage.color = color;
            }

            bool isCanceled = await UniTask.Yield(PlayerLoopTiming.Update, combinedToken).SuppressCancellationThrow();
            if (isCanceled) return;
        }

        Destroy(gameObject);
    }

    private void OnDisable()
    {
        _cts?.Cancel();
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }
}
