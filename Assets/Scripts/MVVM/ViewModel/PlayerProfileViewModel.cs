using System.ComponentModel;
using UnityEngine;

public class PlayerProfileViewModel : ViewModelBase
{
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

    private string _name;
    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }

    private float _totalExp;
    public float TotalExp
    {
        get => _totalExp;
        set
        {
            if (_totalExp != value)
            {
                _totalExp = value;
                OnPropertyChanged(nameof(TotalExp));
            }
        }
    }

    private float _currentLevel;
    public float CurrentLevel
    {
        get => _currentLevel;
        set
        {
            if (_currentLevel != value)
            {
                _currentLevel = value;
                OnPropertyChanged(nameof(CurrentLevel));
            }
        }
    }

    private float _currentHP;
    public float CurrentHP
    {
        get => _currentHP;
        set
        {
            if (_currentHP != value)
            {
                _currentHP = value;
                OnPropertyChanged(nameof(CurrentHP));
            }
        }
    }

    private float _currentMP;
    public float CurrentMP
    {
        get => _currentMP;
        set
        {
            if (_currentMP != value)
            {
                _currentMP = value;
                OnPropertyChanged(nameof(CurrentMP));
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
}
