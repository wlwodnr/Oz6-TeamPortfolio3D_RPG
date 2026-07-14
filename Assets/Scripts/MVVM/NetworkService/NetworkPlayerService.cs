using UnityEngine;

public class NetworkPlayerService
{
    private PlayerModel _playerModel;
    private PlayerProfileViewModel _localPlayerProfileViewModel;
    private PlayerStatViewModel _localPlayerStatViewModel;

    public void Initialize(PlayerModel playerModel)
    {
        _playerModel = playerModel;

        _playerModel.Info.OnInfoChanged += RequestPlayerInfoChanged;
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
        var localPlayerVm = new PlayerProfileViewModel();

        if (_playerModel != null)
        {
            localPlayerVm.Name = _playerModel.Info.Name;
            localPlayerVm.TotalExp = _playerModel.Info.TotalExp;
            localPlayerVm.CurrentLevel = _playerModel.Info.CurLevel;
            localPlayerVm.CurrentHP = _playerModel.Info.CurHp;
            localPlayerVm.CurrentMP = _playerModel.Info.CurMp;
            localPlayerVm.MaxHP = _playerModel.Info.MaxHP;
            localPlayerVm.MaxMP = _playerModel.Info.MaxMP;
        }

        _localPlayerProfileViewModel = localPlayerVm;
        return localPlayerVm;
    }

    private void RequestPlayerInfoChanged(string propertyName)
    {
        var info = _playerModel?.Info;
        if (info == null) return;

        if (_localPlayerProfileViewModel != null)
        {
            switch (propertyName)
            {
                case nameof(PlayerInfo.Name):
                    _localPlayerProfileViewModel.Name = info.Name;
                    break;
                case nameof(PlayerInfo.TotalExp):
                    _localPlayerProfileViewModel.TotalExp = info.TotalExp;
                    break;
                case nameof(PlayerInfo.CurLevel):
                    _localPlayerProfileViewModel.CurrentLevel = info.CurLevel;
                    break;
                case nameof(PlayerInfo.CurHp):
                    _localPlayerProfileViewModel.CurrentHP = info.CurHp;
                    break;
                case nameof(PlayerInfo.CurMp):
                    _localPlayerProfileViewModel.CurrentMP = info.CurMp;
                    break;
                case nameof(PlayerInfo.MaxHP):
                    _localPlayerProfileViewModel.MaxHP = info.MaxHP;
                    break;
                case nameof(PlayerInfo.MaxMP):
                    _localPlayerProfileViewModel.MaxMP = info.MaxMP;
                    break;
            }
        }

        if (_localPlayerStatViewModel != null)
        {
            RequestPlayerStatViewModel(_localPlayerStatViewModel);
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

        if (_playerModel != null)
        {
            RequestPlayerStatViewModel(localPlayerStatVm);
        }

        _localPlayerStatViewModel = localPlayerStatVm;
        return localPlayerStatVm;
    }

    private void RequestPlayerStatViewModel(PlayerStatViewModel vm)
    {
        if (_playerModel?.Info == null || vm == null) return;

        var info = _playerModel.Info;

        vm.AtkDamage = info.AtkDamage;
        vm.MaxHP = info.MaxHP;
        vm.MaxMP = info.MaxMP;
        vm.AtkSpeed = info.AtkSpeed;
        vm.SkillPoint = info.SkillPoint;
    }

    // 아래는 테스트용 함수들 -----------------------------------------------
    public void RequestChangePlayerHp(float hp)
    {
        if (_playerModel != null)
        {
            _playerModel.Info.CurHp += hp;
        }
    }

    public void RequestChangePlayerMp(float mp)
    {
        if (_playerModel != null)
        {
            _playerModel.Info.CurMp += mp;
        }
    }

    public void RequestGiveExpToLocalPlayer(float exp)
    {
        if (_playerModel != null)
        {
            _playerModel.Info.TotalExp += exp;
        }
    }

    public void RequestAddStatAtk(float addAtk)
    {
        if (_playerModel?.Info != null)
        {
            _playerModel.Info.AtkDamage += addAtk;
        }
    }

    public void RequestChangePlayerLevel(int level)
    {
        if (_playerModel?.Info != null)
        {
            _playerModel.Info.CurLevel = level;
        }
    }

    public void RequestChangePlayerMaxHp(float maxHp)
    {
        if (_playerModel?.Info != null)
        {
            _playerModel.Info.MaxHP += maxHp;
        }
    }

    public void RequestChangePlayerMaxMp(float maxMp)
    {
        if (_playerModel?.Info != null)
        {
            _playerModel.Info.MaxMP += maxMp;
        }
    }

    public void RequestChangePlayerName(string newName)
    {
        if (_playerModel?.Info != null)
        {
            _playerModel.Info.Name = newName;
        }
    }
}
