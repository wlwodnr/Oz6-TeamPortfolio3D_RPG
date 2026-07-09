using UnityEngine;

public class NetworkPlayerService
{
    private PlayerProfileViewModel _localPlayerProfileViewModel;
    private PlayerStatViewModel _localPlayerStatViewModel;

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
        var localPlayerVm = new PlayerProfileViewModel
        {
            Name = "기본 이름",
            TotalExp = 0,
            CurrentLevel = 0,
            CurrentHP = 10,
            CurrentMP = 10
        };

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

    public void RequestChangePlayerLevel(int level)
    {
        if (_localPlayerProfileViewModel != null)
        {
            _localPlayerProfileViewModel.CurrentLevel = level;
        }
    }

    public void RequestChangePlayerHp(int hp)
    {
        if (_localPlayerProfileViewModel != null)
        {
            _localPlayerProfileViewModel.CurrentHP = hp;
        }
    }

    public void RequestChangePlayerMp(int mp)
    {
        if (_localPlayerProfileViewModel != null)
        {
            _localPlayerProfileViewModel.CurrentMP = mp;
        }
    }

    public void RequestChangePlayerMaxHp(int maxHp)
    {
        if (_localPlayerProfileViewModel != null)
        {
            _localPlayerProfileViewModel.MaxHP = maxHp;
        }
    }

    public void RequestChangePlayerMaxMp(int maxMp)
    {
        if (_localPlayerProfileViewModel != null)
        {
            _localPlayerProfileViewModel.MaxMP = maxMp;
        }
    }

    public void RequestChangePlayerName(string newName)
    {
        if (_localPlayerProfileViewModel != null)
        {
            _localPlayerProfileViewModel.Name = newName;
        }
    }

    public PlayerStatViewModel GetLocalPlayerStatViewModel()
    {
        if (_localPlayerStatViewModel == null)
        {
            var localPlayerStatVm = new PlayerStatViewModel();
            SetPlayerStatData(localPlayerStatVm, atk: 10, hp: 100, mp: 100, atkSpeed: 1, skillPoint: 10);

            var playerStatData = GameDataManager.Instance.GetPlayerStatData("stat_dummy");
            if (playerStatData == null)
            {
                Debug.LogError("플레이어 데이터를 찾을 수 없습니다. : NetworkPlayerService");
                _localPlayerStatViewModel = localPlayerStatVm;
                return _localPlayerStatViewModel;
            }

            SetPlayerStatData(localPlayerStatVm, playerStatData.Atk, playerStatData.HP,
                playerStatData.MP, playerStatData.AtkSpeed, playerStatData.SkillPoint);

            // 스탯 데이터의 HP/MP를 프로필 뷰모델의 MaxHP / MaxMP로 동기화
            if (_localPlayerProfileViewModel != null)
            {
                _localPlayerProfileViewModel.MaxHP = playerStatData.HP;
                _localPlayerProfileViewModel.MaxMP = playerStatData.MP;
            }

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

    // 아래는 테스트용 함수들
    public void RequestAddStatAtk(int addAtk)
    {
        if (_localPlayerStatViewModel != null)
        {
            _localPlayerStatViewModel.Atk += addAtk;
        }
    }

    public void RequestAddStatHP(int addHp)
    {
        if (_localPlayerStatViewModel != null)
        {
            _localPlayerStatViewModel.HP += addHp;
            _localPlayerProfileViewModel.CurrentHP += addHp;
        }
    }

    public void RequestAddStatMP(int addMp)
    {
        if (_localPlayerStatViewModel != null)
        {
            _localPlayerStatViewModel.MP += addMp;
            _localPlayerProfileViewModel.CurrentMP += addMp;
        }
    }

}
