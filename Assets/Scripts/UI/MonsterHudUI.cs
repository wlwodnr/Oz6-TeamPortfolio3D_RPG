using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class MonsterHudUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Slider Slider_HpBar;
    [SerializeField] private TextMeshProUGUI TextMesh_Hp;

    [Header("Position Tracking")]
    [SerializeField] private Vector3 _offset = new Vector3(0f, 2.0f, 0f);

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
            transform.position = _mainCamera.WorldToScreenPoint(_targetTransform.position + _offset); // 씬 스페이스 오버레이만 가능
        }
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

        if (TextMesh_Hp != null)
        {
            TextMesh_Hp.text = $"{Mathf.CeilToInt(_vm.CurrentHp)} / {Mathf.CeilToInt(_vm.MaxHP)}";
        }
        TextMesh_Hp.gameObject.SetActive(false); // 체력 텍스트 비활성화
    }
}
