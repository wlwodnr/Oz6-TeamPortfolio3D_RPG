using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileUI : UIBase
{
    [SerializeField] Text Text_Name;
    [SerializeField] Text Text_TotalExp;
    [SerializeField] Text Text_Level;
    [SerializeField] Text Text_CurrentHP;
    [SerializeField] Text Text_CurrentMP;
    [SerializeField] UIButton Btn_OpenStatInfoUI;

    // 뷰에서 절대 new로 VewModel을 하지 않고, 네트워크 매니저를 통해 생성된 뷰 모델을 받아야 한다
    private PlayerProfileViewModel _vm;

    private void OnEnable()
    {
        Btn_OpenStatInfoUI.BindOnClickButtonEvent(OnClick_OpenPlayerStatInfoUI);
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
                    Text_TotalExp.text = $"({_vm.TotalExp})";
                }
            break;
            case nameof(PlayerProfileViewModel.CurrentLevel):
            {
                Text_Level.text = $"Lv.{_vm.CurrentLevel}";
                //ChangeAnimationOnSuccessLevelUp();
            }
            break;
            case nameof(PlayerProfileViewModel.CurrentHP):
            {
                    Text_CurrentHP.text = $"{_vm.CurrentHP}";
                }
            break;
            case nameof(PlayerProfileViewModel.CurrentMP):
                {
                    Text_CurrentMP.text = $"{_vm.CurrentMP}";
                }
            break;
        }
    }

}
