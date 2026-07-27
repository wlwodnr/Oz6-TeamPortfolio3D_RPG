using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class LoadingUI : UIBase
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Text _text;

    private Action _onLoadingComplete;

    private void OnEnable()
    {
        StartLoading();
    }

    public void StartLoading(Action onComplete = null)
    {
        _onLoadingComplete = onComplete;

        LoadingRoutineAsync().Forget();
    }

    private async UniTaskVoid LoadingRoutineAsync()
    {
        float duration = 2f;
        float elapsed = 0f;

        if (_slider != null) _slider.value = 0f;

        var cancellationToken = this.GetCancellationTokenOnDestroy();

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);

            if (_slider != null)
            {
                _slider.value = progress;
            }
            if (_text != null)
            {
                _text.text = $"Loading... {(int)(progress * 100)}%";
            }
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }

        if (_slider != null) { _slider.value = 1f; }
        if (_text != null) { _text.text = "Completed!"; }

        await UniTask.Delay(TimeSpan.FromSeconds(0.5f), ignoreTimeScale: true, cancellationToken: cancellationToken);

        _onLoadingComplete?.Invoke();

        UIManager.Instance.CloseUI(UIRootType.VeryFrontUI, UIType.LoadingUI);
    }
}
