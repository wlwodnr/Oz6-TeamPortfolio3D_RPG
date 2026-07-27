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
        _playerModel.Info.CurHp += hp;
    }

    public void RequestDamagePlayerHp(float dmg)
    {
        _playerModel.Info.CurHp -= dmg;
    }

    public void RequestChangePlayerMp(float mp)
    {
        _playerModel.Info.CurMp += mp;
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
    //    _playerModel.ModifyBaseStat(StatType.MaxHP, maxHp);
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

    public float GetPlayerDefense()
    {
        return _playerModel.GetStatValue(StatType.Defense);
    }

    public void HandlePlayerDead()
    {
        // 여기서 처리 순서가 상관없다면 이벤트로 쏘고, 아니라면 여기서 전부 순서대로 처리
    }
}
