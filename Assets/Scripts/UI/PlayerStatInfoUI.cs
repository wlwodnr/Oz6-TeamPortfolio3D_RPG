using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatInfoUI : UIBase
{
    [SerializeField] Text Text_Atk;
    [SerializeField] Text Text_HP;
    [SerializeField] Text Text_MP;
    [SerializeField] Text Text_AtkSpeed;
    [SerializeField] Text Text_SkillPoint;
    [SerializeField] UIButton Btn_Close;

    // 뷰에서 절대 new로 VewModel을 하지 않고, 네트워크 매니저를 통해 생성된 뷰 모델을 받아야 한다
    private PlayerStatViewModel _statVm;

    private void OnEnable()
    {
        Btn_Close.BindOnClickButtonEvent(OnClick_ClosePopup);

        var statVm = NetworkManager.Inst.LocalPlayerService.GetLocalPlayerStatViewModel();
        if (statVm != null)
        {
            BindViewModel(statVm);
        }
    }

    private void OnClick_ClosePopup()
    {
        UIManager.Instance.ClosePlayerStatInfoUI();
    }
     
    public void BindViewModel(PlayerStatViewModel vm)
    {
        _statVm = vm;
        _statVm.PropertyChanged += OnPropChanged_StatView;
        _statVm.InvokeOnceOnInit();
    }

    private void OnDestroy()
    {
        if (_statVm != null)
        {
            _statVm.PropertyChanged -= OnPropChanged_StatView;
        }
    }

    private void OnPropChanged_StatView(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(PlayerStatViewModel.AtkDamage):
                {
                    Text_Atk.text = $"Atk : {_statVm.AtkDamage}";
                }
            break;
            case nameof(PlayerStatViewModel.MaxHP):
                {
                    Text_HP.text = $"HP : {_statVm.MaxHP}";

                }
            break;
            case nameof(PlayerStatViewModel.MaxMP):
                {
                    Text_MP.text = $"MP : {_statVm.MaxMP}";

                }
            break;
            case nameof(PlayerStatViewModel.AtkSpeed):
                {
                    Text_AtkSpeed.text = $"AtkSpd : {_statVm.AtkSpeed}";
                }
            break;
            case nameof(PlayerStatViewModel.SkillPoint):
                {
                    Text_SkillPoint.text = $"Skill Point : {_statVm.SkillPoint}";

                }
            break;
        }
    }
}
