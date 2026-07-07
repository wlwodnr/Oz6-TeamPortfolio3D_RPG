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
    
    // 뷰에서 절대 new로 VewModel을 하지 않고, 네트워크 매니저를 통해 생성된 뷰 모델을 받아야 한다
    private PlayerStatViewModel _statVm;

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
            case nameof(PlayerStatViewModel.Atk):
                {
                    Text_Atk.text = $"Atk : {_statVm.Atk}";
                }
                break;
            case nameof(PlayerStatViewModel.HP):
                {
                    Text_HP.text = $"Crit : {_statVm.HP}";

                }
                break;
            case nameof(PlayerStatViewModel.MP):
                {
                    Text_MP.text = $"Hp : {_statVm.MP}";

                }
                break;
            case nameof(PlayerStatViewModel.SkillPoint):
                {
                    Text_AtkSpeed.text = $"Spd : {_statVm.SkillPoint}";

                }
                break;
            case nameof(PlayerStatViewModel.AtkSpeed):
                {
                    Text_SkillPoint.text = $"AtkSpd : {_statVm.AtkSpeed}%";
                }
                break;
        }
    }
}
