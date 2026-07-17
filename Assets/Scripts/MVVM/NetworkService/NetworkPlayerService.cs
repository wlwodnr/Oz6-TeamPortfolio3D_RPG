using UnityEngine;

public class NetworkPlayerService
{
    private PlayerModel _playerModel;
    private PlayerProfileViewModel _localPlayerProfileViewModel;
    private PlayerStatViewModel _localPlayerStatViewModel;

    public void Initialize(PlayerModel playerModel)
    {
        _playerModel = playerModel;
    }

    public PlayerProfileViewModel GetLocalPlayerProfileViewModel()
    {
        if(_localPlayerProfileViewModel == null)
        {
            CreateLocalPlayerProfileViewModel();
        }

        return _localPlayerProfileViewModel;
    }

    public PlayerProfileViewModel CreateLocalPlayerProfileViewModel()
    {
        _localPlayerProfileViewModel = new PlayerProfileViewModel(_playerModel);
        return _localPlayerProfileViewModel;
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
        _localPlayerStatViewModel = new PlayerStatViewModel(_playerModel);
        return _localPlayerStatViewModel;
    }

    // 아래는 테스트용 함수들 -----------------------------------------------
    public void RequestChangePlayerHp(float hp)
    {
        var vm = GetLocalPlayerProfileViewModel();
        if (vm != null)
        {
            vm.CurrentHP += hp;
        }
    }

    public void RequestChangePlayerMp(float mp)
    {
        var vm = GetLocalPlayerProfileViewModel();
        if (vm != null)
        {
            vm.CurrentMP += mp;
        }
    }

    public void RequestGiveExpToLocalPlayer(float exp)
    {
        var vm = GetLocalPlayerProfileViewModel();
        if (vm != null)
        {
            vm.TotalExp += exp;
        }
    }

    public void RequestAddStatAtk(float addAtk)
    {
        var vm = GetLocalPlayerStatViewModel();
        if (vm != null)
        {
            vm.AtkDamage += addAtk;
        }
    }

    public void RequestChangePlayerLevel(int level)
    {
        var vm = GetLocalPlayerProfileViewModel();
        if (vm != null)
        {
            vm.CurrentLevel = level;
        }
    }

    public void RequestChangePlayerMaxHp(float maxHp)
    {
        var vm = GetLocalPlayerProfileViewModel();
        if (vm != null)
        {
            vm.MaxHP += maxHp;
        }
    }

    public void RequestChangePlayerMaxMp(float maxMp)
    {
        var vm = GetLocalPlayerProfileViewModel();
        if (vm != null)
        {
            vm.MaxMP += maxMp;
        }
    }

    public void RequestChangePlayerName(string newName)
    {
        var vm = GetLocalPlayerProfileViewModel();
        if (vm != null)
        {
            vm.Name = newName;
        }
    }
}
