using System.ComponentModel;
using UnityEngine;

public class PlayerProfileViewModel : ViewModelBase
{
    private PlayerModel _playerModel;

    public PlayerProfileViewModel(PlayerModel playerModel)
    {
        _playerModel = playerModel;

        _playerModel.OnPlayerStatsChanged += HandleStatsChanged;
        _playerModel.OnPlayerInfoChanged += HandleInfoChanged;
    }

    public void InvokeOnceOnInit()
    {
        OnPropertyChanged(nameof(Name));
        OnPropertyChanged(nameof(TotalExp));
        OnPropertyChanged(nameof(CurrentLevel));
        OnPropertyChanged(nameof(CurrentHP));
        OnPropertyChanged(nameof(CurrentMP));
        OnPropertyChanged(nameof(MaxHP));
        OnPropertyChanged(nameof(MaxMP));
    }

    private void HandleStatsChanged(string changedType)
    {
        if (changedType == nameof(StatType.MaxHP))
        {
            OnPropertyChanged(nameof(MaxHP));
        }
        else if (changedType == nameof(StatType.MaxMP))
        {
            OnPropertyChanged(nameof(MaxMP));
        }
    }

    private void HandleInfoChanged(string changedType)
    {
        switch (changedType)
        {
            case nameof(PlayerInfo.Name):
                OnPropertyChanged(nameof(Name));
                break;
            case nameof(PlayerInfo.TotalExp):
                OnPropertyChanged(nameof(TotalExp));
                break;
            case nameof(PlayerInfo.CurLevel):
                OnPropertyChanged(nameof(CurrentLevel));
                break;
            case nameof(PlayerInfo.CurHp):
                OnPropertyChanged(nameof(CurrentHP));
                break;
            case nameof(PlayerInfo.CurMp):
                OnPropertyChanged(nameof(CurrentMP));
                break;
        }
    }

    public string Name
    {
        get => _playerModel.Info.Name;
        set { if (_playerModel.Info.Name != value) _playerModel.Info.Name = value; }
    }

    public float TotalExp
    {
        get => _playerModel.Info.TotalExp;
        set { if (_playerModel.Info.TotalExp != value) _playerModel.Info.TotalExp = value; }
    }

    public int CurrentLevel
    {
        get => _playerModel.Info.CurLevel;
        set { if (_playerModel.Info.CurLevel != value) _playerModel.Info.CurLevel = value; }
    }

    public float CurrentHP
    {
        get => _playerModel.Info.CurHp;
        set { if (_playerModel.Info.CurHp != value) _playerModel.Info.CurHp = value; }
    }

    public float CurrentMP
    {
        get => _playerModel.Info.CurMp;
        set { if (_playerModel.Info.CurMp != value) _playerModel.Info.CurMp = value; }
    }

    public float MaxHP => _playerModel.GetStatValue(StatType.MaxHP);
    public float MaxMP => _playerModel.GetStatValue(StatType.MaxMP);
}
