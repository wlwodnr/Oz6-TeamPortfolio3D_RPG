using UnityEngine;

public class NetworkPlayerService
{
    private PlayerProfileViewModel _localPlayerProfileViewModel;
    private PlayerStatViewModel _localPlayerStatViewModel;

    public PlayerProfileViewModel GetLocalPlayerViewModel()
    {
        if(_localPlayerProfileViewModel == null)
        {
            CreateLocalPlayerProfileViewModel();
        }

        return _localPlayerProfileViewModel;
    }

    public PlayerProfileViewModel CreateLocalPlayerProfileViewModel()
    {
        var localPlayerVm = new PlayerProfileViewModel();
        localPlayerVm.Name = "기본 이름";
        localPlayerVm.TotalExp = 0;
        _localPlayerProfileViewModel = localPlayerVm; 
        return localPlayerVm;
    }

    public void RequestGiveExpToLocalPlayer(int exp)
    {
        if (_localPlayerProfileViewModel != null)
        {
            _localPlayerProfileViewModel.TotalExp += exp;
        }
    }

    public void RequestChangePlayerName(string newName)
    {
        if (_localPlayerProfileViewModel != null)
        {
            _localPlayerProfileViewModel.Name = newName;
        }
    }

    public void RequestAddStatAtk(int addAtk)
    {
        if (_localPlayerStatViewModel != null)
        {
            _localPlayerStatViewModel.Atk += addAtk;
        }
    }

    public PlayerStatViewModel GetLocalPlayerStatViewModel()
    {
        if (_localPlayerStatViewModel == null)
        {
            CreateLocalPlayerStatViewModel();
        }

        return _localPlayerStatViewModel;
    }

    public PlayerStatViewModel CreateLocalPlayerStatViewModel()
    {

        var localPlayerStatVm = new PlayerStatViewModel();
        localPlayerStatVm.Atk = 0;
        localPlayerStatVm.HP = 100;
        localPlayerStatVm.MP = 100;
        localPlayerStatVm.AtkSpeed = 1;
        localPlayerStatVm.SkillPoint = 1;
        _localPlayerStatViewModel = localPlayerStatVm;
        return localPlayerStatVm;
    }

}
