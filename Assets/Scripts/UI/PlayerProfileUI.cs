using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileUI : UIBase
{
    [Header("Slider")]
    [SerializeField] Slider hpBar;
    [SerializeField] Slider mpBar;
    [SerializeField] Slider staminaBar;
    [SerializeField] Slider expBar;

    [Header("Text")]
    [SerializeField] Text Text_Name;
    [SerializeField] Text Text_LevelAndExp;
    [SerializeField] Text Text_CurrentHP;
    [SerializeField] Text Text_CurrentMP;
    [SerializeField] UIButton Btn_OpenStatInfoUI;

    // 뷰에서 절대 new로 VewModel을 하지 않고, 네트워크 매니저를 통해 생성된 뷰 모델을 받아야 한다
    private PlayerProfileViewModel _vm;

    private const float MAX_EXP_PER_LEVEL = 100f;

    private void OnEnable()
    {
        Btn_OpenStatInfoUI.BindOnClickButtonEvent(OnClick_OpenPlayerStatInfoUI);

        var profileVm = NetworkManager.Inst.LocalPlayerService.GetLocalPlayerProfileModel();
        if (profileVm != null)
        {
            BindViewModel(profileVm);
        }
    }

    private void OnClick_OpenPlayerStatInfoUI()
    {
        UIManager.Instance.OpenPlayerStatInfoUI();
    }

    public void BindViewModel(PlayerProfileViewModel vm)
    {
        _vm = vm;
        _vm.PropertyChanged += OnPropChagned_View;
        _vm.InvokeOnceOnInit();
    }

    private void OnDestroy()
    {
        if (_vm != null)
        {
            _vm.PropertyChanged -= OnPropChagned_View;
        }
    }

    private void OnPropChagned_View(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(PlayerProfileViewModel.Name):
                {
                    Text_Name.text = _vm.Name;
                }
                break;
            case nameof(PlayerProfileViewModel.TotalExp):
                {
                    Text_LevelAndExp.text = $"Lv.{_vm.CurrentLevel}({_vm.TotalExp})";
                    UpdateExpBar();
                }
                break;
            case nameof(PlayerProfileViewModel.CurrentHP):
            case nameof(PlayerProfileViewModel.MaxHP):
                {
                    Text_CurrentHP.text = $"{_vm.CurrentHP}/{_vm.MaxHP}";
                    UpdateHpBar();
                }
                break;
            case nameof(PlayerProfileViewModel.CurrentMP):
            case nameof(PlayerProfileViewModel.MaxMP):
                {
                    Text_CurrentMP.text = $"{_vm.CurrentMP}/{_vm.MaxMP}";
                    UpdateMpBar();
                }
                break;
        }
    }

    private void UpdateHpBar()
    {
        if (hpBar != null && _vm.MaxHP > 0)
        {
            hpBar.value = (float)_vm.CurrentHP / _vm.MaxHP;
        }
    }

    private void UpdateMpBar()
    {
        if (mpBar != null && _vm.MaxMP > 0)
        {
            mpBar.value = (float)_vm.CurrentMP / _vm.MaxMP;
        }
    }

    private void UpdateExpBar()
    {
        if (expBar != null && _vm != null)
        {
            float currentLevelExp = _vm.TotalExp % MAX_EXP_PER_LEVEL;
            expBar.value = currentLevelExp / MAX_EXP_PER_LEVEL;
        }
    }
}
