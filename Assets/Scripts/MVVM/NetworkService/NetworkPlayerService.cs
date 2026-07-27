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

    public PlayerModel GetLocalPlayerModel()
    {
        if( _playerModel == null)
        {
            _playerModel = new PlayerModel();
        }

        return _playerModel;
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

    public void RequestAddItem(string itemId)
    {
        _playerModel?.Additem(itemId);
    }

    public void RequestChangePlayerMp(float mp)
    {
        _playerModel?.ChangeMp(mp);
    }

    public void RequestGiveExpToLocalPlayer(float exp)
    {
        if (exp <= 0f || float.IsNaN(exp) || float.IsInfinity(exp))
        {
            Debug.LogWarning($"지급할 경험치는 0보다 커야 합니다. Experience: {exp}");
            return;
        }

        GetLocalPlayerModel().AddExperience(exp);
    }

    public void RequestGiveGoldToLocalPlayer(int gold)
    {
        if (gold <= 0)
        {
            Debug.LogWarning($"지급할 골드는 0보다 커야 합니다. Gold: {gold}");
            return;
        }

        GetLocalPlayerModel().AddGold(gold);
    }

    public void RequestChangePlayerLevel(int level)
    {
        GetLocalPlayerModel().Info.CurLevel = level;
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

    public float GetPlayerDefense()
    {
        return _playerModel.GetStatValue(StatType.Defense);
    }

    public void HandlePlayerDead()
    {
        // 여기서 처리 순서가 상관없다면 이벤트로 쏘고, 아니라면 여기서 전부 순서대로 처리
    }
}
