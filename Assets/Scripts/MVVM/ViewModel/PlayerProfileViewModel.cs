using System.ComponentModel;
using UnityEngine;

public class PlayerProfileViewModel : ViewModelBase
{
    private PlayerModel _playerModel;

    public PlayerProfileViewModel(PlayerModel playerModel)
    {
        _playerModel = playerModel;
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

    public string Name
    {
        get => _playerModel.Info.Name;
        set
        {
            if (_playerModel.Info.Name != value)
            {
                _playerModel.Info.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }

    public float TotalExp
    {
        get => _playerModel.Info.TotalExp;
        set
        {
            if (_playerModel.Info.TotalExp != value)
            {
                _playerModel.Info.TotalExp = value;
                OnPropertyChanged(nameof(TotalExp));
            }
        }
    }

    public int CurrentLevel
    {
        get => _playerModel.Info.CurLevel;
        set
        {
            if (_playerModel.Info.CurLevel != value)
            {
                _playerModel.Info.CurLevel = value;
                OnPropertyChanged(nameof(CurrentLevel));
            }
        }
    }

    public float CurrentHP
    {
        get => _playerModel.Info.CurHp;
        set
        {
            if (_playerModel.Info.CurHp != value)
            {
                _playerModel.Info.CurHp = value;
                OnPropertyChanged(nameof(CurrentHP));
            }
        }
    }

    public float CurrentMP
    {
        get => _playerModel.Info.CurMp;
        set
        {
            if (_playerModel.Info.CurMp != value)
            {
                _playerModel.Info.CurMp = value;
                OnPropertyChanged(nameof(CurrentMP));
            }
        }
    }

    public float MaxHP
    {
        get => _playerModel.Info.MaxHP;
        set
        {
            if (_playerModel.Info.MaxHP != value)
            {
                _playerModel.Info.MaxHP = value;
                OnPropertyChanged(nameof(MaxHP));
            }
        }
    }

    public float MaxMP
    {
        get => _playerModel.Info.MaxMP;
        set
        {
            if (_playerModel.Info.MaxMP != value)
            {
                _playerModel.Info.MaxMP = value;
                OnPropertyChanged(nameof(MaxMP));
            }
        }
    }
}
