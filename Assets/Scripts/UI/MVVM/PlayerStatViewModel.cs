using UnityEngine;

public class PlayerStatViewModel : ViewModelBase
{
    public void InvokeOnceOnInit()
    {
        OnPropertyChanged(nameof(Atk));
        OnPropertyChanged(nameof(HP));
        OnPropertyChanged(nameof(MP));
        OnPropertyChanged(nameof(AtkSpeed));
        OnPropertyChanged(nameof(SkillPoint));
    }

    private int _atk;
    public int Atk
    {
        get => _atk;
        set
        {
            if (_atk != value)
            {
                _atk = value;
                OnPropertyChanged(nameof(Atk));
            }
        }
    }

    private int _Hp;
    public int HP
    {
        get => _Hp;
        set
        {
            if (_Hp != value)
            {
                _Hp = value;
                OnPropertyChanged(nameof(HP));
            }
        }
    }

    private int _Mp;
    public int MP
    {
        get => _Mp;
        set
        {
            if (_Mp != value)
            {
                _Mp = value;
                OnPropertyChanged(nameof(MP));
            }
        }
    }

    private int _atkSpeed;
    public int AtkSpeed
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
