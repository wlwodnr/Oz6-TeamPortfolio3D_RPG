using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHudUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Slider Slider_HpBar;
    [SerializeField] private Text Text_Hp;

    [Header("Position Tracking")]
    [SerializeField] private Vector3 _offset = new Vector3(0f, 2.0f, 0f);

    [Header("Distance & Scale Settings")]
    [SerializeField] private float _maxVisibleDistance = 30f;
    [SerializeField] private float _referenceDistance = 10f;
    [SerializeField] private float _minScale = 0.5f;
    [SerializeField] private float _maxScale = 1.2f;

    private MonsterViewModel _vm;
    private Transform _targetTransform;
    private Camera _mainCamera;

    private void LateUpdate()
    {
        if (_mainCamera == null) 
        {
            _mainCamera = Camera.main; 
        }
        // 몬스터 위치 추적
        if (_targetTransform != null && _mainCamera != null)
        {
            Vector3 targetWorldPos = _targetTransform.position + _offset;
            Vector3 screenPos = _mainCamera.WorldToScreenPoint(targetWorldPos);

            float distance = Vector3.Distance(_mainCamera.transform.position, targetWorldPos);

            if (screenPos.z <= 0f || distance > _maxVisibleDistance)
            {
                if (gameObject.activeSelf) gameObject.SetActive(false);
                return;
            }

            if (!gameObject.activeSelf) 
            {
                gameObject.SetActive(true); 
            }

            transform.position = screenPos;

            UpdateScaleByDistance(distance);
        }
    }

    // 거리에 따른 HUD 크기(Scale) 보정
    private void UpdateScaleByDistance(float distance)
    {
        float scaleFactor = _referenceDistance / Mathf.Max(distance, 0.001f);

        float finalScale = Mathf.Clamp(scaleFactor, _minScale, _maxScale);

        transform.localScale = Vector3.one * finalScale;
    }

    private void OnDestroy()
    {
        UnbindViewModel();
    }

    public void BindViewModel(MonsterViewModel vm, Transform targetTransform)
    {
        UnbindViewModel();

        _vm = vm;
        _targetTransform = targetTransform;

        if (_vm != null)
        {
            _vm.PropertyChanged += OnPropChanged_View;
            _vm.InvokeOnceOnInit();
        }
    }

    private void UnbindViewModel()
    {
        if (_vm != null)
        {
            _vm.PropertyChanged -= OnPropChanged_View;
            _vm = null;
        }
        _targetTransform = null;
    }

    private void OnPropChanged_View(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(MonsterViewModel.CurrentHp):
            case nameof(MonsterViewModel.MaxHP):
                UpdateHpUI();
                break;

            case nameof(MonsterViewModel.IsDead):
                if (_vm.IsDead)
                {
                    UnbindViewModel();
                    gameObject.SetActive(false);
                }
                break;
        }
    }

    private void UpdateHpUI()
    {
        if (_vm == null) return;

        if (Slider_HpBar != null && _vm.MaxHP > 0f)
        {
            Slider_HpBar.value = _vm.CurrentHp / _vm.MaxHP;
        }

        if (Text_Hp != null)
        {
            Text_Hp.text = $"{Mathf.CeilToInt(_vm.CurrentHp)} / {Mathf.CeilToInt(_vm.MaxHP)}";
        }
        Text_Hp.gameObject.SetActive(false); // 체력 텍스트 비활성화, 활성화시 제거
    }
}
