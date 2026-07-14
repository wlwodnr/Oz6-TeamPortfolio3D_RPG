using UnityEngine;

public class PlayerStatViewModel : ViewModelBase
{
    public void InvokeOnceOnInit()
    {
        OnPropertyChanged(nameof(AtkDamage));
        OnPropertyChanged(nameof(MaxHP));
        OnPropertyChanged(nameof(MaxMP));
        OnPropertyChanged(nameof(AtkSpeed));
        OnPropertyChanged(nameof(SkillPoint));
    }

    private float _atkDamage;
    public float AtkDamage
    {
        get => _atkDamage;
        set
        {
            if (_atkDamage != value)
            {
                _atkDamage = value;
                OnPropertyChanged(nameof(AtkDamage));
            }
        }
    }

    private float _maxHP;

    public float MaxHP
    {
        get => _maxHP;
        set
        {
            if (_maxHP != value)
            {
                _maxHP = value;
                OnPropertyChanged(nameof(MaxHP));
            }
        }
    }

    private float _maxMP;
    public float MaxMP
    {
        get => _maxMP;
        set
        {
            if (_maxMP != value)
            {
                _maxMP = value;
                OnPropertyChanged(nameof(MaxMP));
            }
        }
    }

    private float _atkSpeed;
    public float AtkSpeed
    {
        get => _atkSpeed;
        set
        {
            if (_atkSpeed != value)
            {
                _atkSpeed = value;
                OnPropertyChanged(nameof(AtkSpeed));
            }
        }
    }

    private int _skillPoint;
    public int SkillPoint
    {
        get => _skillPoint;
        set
        {
            if (_skillPoint != value)
            {
                _skillPoint = value;
                OnPropertyChanged(nameof(SkillPoint));
            }
        }
    }
}
