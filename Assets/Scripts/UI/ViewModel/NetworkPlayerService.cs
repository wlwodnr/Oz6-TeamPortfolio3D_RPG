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
            var localPlayerStatVm = new PlayerStatViewModel();
            SetPlayerStatData(localPlayerStatVm, atk: 10, hp: 100, mp: 100, atkSpeed: 1, skillPoint: 10);

            var playerStatData = GameDataManager.Instance.GetData<PlayerStatData>("stat_dummy");
            if (playerStatData == null)
            {
                Debug.LogError("플레이어 데이터를 찾을 수 없습니다. : NetworkPlayerService");
                _localPlayerStatViewModel = localPlayerStatVm;
                return _localPlayerStatViewModel;
            }

            SetPlayerStatData(localPlayerStatVm, playerStatData.Atk, playerStatData.HP,
                playerStatData.MP, playerStatData.AtkSpeed, playerStatData.SkillPoint);

            _localPlayerStatViewModel = localPlayerStatVm;
        }

        return _localPlayerStatViewModel;
    }

    private void SetPlayerStatData(PlayerStatViewModel vm, int atk, int hp, int mp, int atkSpeed, int skillPoint)
    {
        vm.Atk = atk;
        vm.HP = hp;
        vm.MP = mp;
        vm.AtkSpeed = atkSpeed;
        vm.SkillPoint = skillPoint;
    }
}
