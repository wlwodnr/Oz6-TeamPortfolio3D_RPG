using UnityEngine;

public class PlayerStatViewModel : ViewModelBase
{
    private PlayerModel _playerModel;

    public PlayerStatViewModel(PlayerModel playerModel)
    {
        _playerModel = playerModel;
    }

    public void InvokeOnceOnInit()
    {
        OnPropertyChanged(nameof(AtkDamage));
        OnPropertyChanged(nameof(MaxHP));
        OnPropertyChanged(nameof(MaxMP));
        OnPropertyChanged(nameof(AtkSpeed));
        OnPropertyChanged(nameof(SkillPoint));
    }

    public float AtkDamage
    {
        get => _playerModel.Info.AtkDamage;
        set
        {
            if (_playerModel.Info.AtkDamage != value)
            {
                _playerModel.Info.AtkDamage = value;
                OnPropertyChanged(nameof(AtkDamage));
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

    public float AtkSpeed
    {
        get => _playerModel.Info.AtkSpeed;
        set
        {
            if (_playerModel.Info.AtkSpeed != value)
            {
                _playerModel.Info.AtkSpeed = value;
                OnPropertyChanged(nameof(AtkSpeed));
            }
        }
    }

    public int SkillPoint
    {
        get => _playerModel.Info.SkillPoint;
        set
        {
            if (_playerModel.Info.SkillPoint != value)
            {
                _playerModel.Info.SkillPoint = value;
                OnPropertyChanged(nameof(SkillPoint));
            }
        }
    }
}
