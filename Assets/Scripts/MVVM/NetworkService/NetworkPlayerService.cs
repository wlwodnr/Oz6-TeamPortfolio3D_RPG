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

    public PlayerProfileViewModel GetLocalPlayerProfileModel()
    {
        if(_localPlayerProfileViewModel == null)
        {
            CreateLocalPlayerProfileModel();
        }

        return _localPlayerProfileViewModel;
    }

    public PlayerProfileViewModel CreateLocalPlayerProfileModel()
    {
        _localPlayerProfileViewModel = new PlayerProfileViewModel(_playerModel);
        return _localPlayerProfileViewModel;
    }

    // 플레이어 스탯뷰모델
    public PlayerStatViewModel GetLocalPlayerStatModel()
    {
        if (_localPlayerStatViewModel == null)
        {
            CreateLocalPlayerStatModel();
        }

        return _localPlayerStatViewModel;
    }

    public PlayerStatViewModel CreateLocalPlayerStatModel()
    {
        _localPlayerStatViewModel = new PlayerStatViewModel(_playerModel);
        return _localPlayerStatViewModel;
    }


    public void RequestChangePlayerHp(float hp)
    {
        _playerModel?.ChangeHp(hp);
    }

    public void RequestChangePlayerMp(float mp)
    {
        _playerModel?.ChangeMp(mp);
    }

    public void RequestGiveExpToLocalPlayer(float exp)
    {
        _playerModel.Info.TotalExp += exp;
    }

    public void RequestChangePlayerLevel(int level)
    {
        _playerModel.Info.CurLevel = level;
    }

    public void RequestChangePlayerName(string newName)
    {
        _playerModel.Info.Name = newName;
    }

    //public void RequestChangePlayerMaxHp(float maxHp)
    //{
    //    _playerModel.Stats.BaseStats[StatType.MaxHP] = maxHp;
    //    _playerModel.Stats.NotifyStatsUpdated(StatType.MaxHP.ToString());
    //}

    //public void RequestChangePlayerMaxMp(float maxMp)
    //{
    //    _playerModel.ModifyBaseStat(StatType.MaxMP, maxMp);
    //}

    //public void RequestAddStatAtkPower(float addAtk)
    //{
    //    _playerModel.ModifyBaseStat(StatType.AttackPower, addAtk);
    //}
    //public void RequestAddStatAtkSpeed(float addAtkSpeed)
    //{
    //    _playerModel.ModifyBaseStat(StatType.AttackSpeed, addAtk);
    //}
}
