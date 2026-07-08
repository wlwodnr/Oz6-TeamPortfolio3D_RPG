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

    private int _totalExp;
    public int TotalExp
    {
        get => _totalExp;
        set
        {
            if (_totalExp != value)
            {
                _totalExp = value;
                CurrentLevel = (int)_totalExp / 100;
                OnPropertyChanged(nameof(TotalExp));
            }
        }
    }

    private int _currentLevel;
    public int CurrentLevel
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

    private int _currentHP;
    public int CurrentHP
    {
        get => _currentHP;
        set
        {
            if (_currentHP != value)
            {
                _currentHP = value;
                OnPropertyChanged(nameof(_currentHP));
            }
        }
    }

    private int _currentMP;
    public int CurrentMP
    {
        get => _currentMP;
        set
        {
            if (_currentMP != value)
            {
                _currentMP = value;
                OnPropertyChanged(nameof(_currentMP));
            }
        }
    }
}
