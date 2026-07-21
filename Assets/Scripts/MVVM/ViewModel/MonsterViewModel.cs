using System;

public class MonsterViewModel : ViewModelBase
{
    private MonsterModel _monsterModel;

    public MonsterViewModel(MonsterModel monsterModel)
    {
        _monsterModel = monsterModel;
        _monsterModel.OnHpChanged += HandleHpChanged;
        _monsterModel.OnDied += HandleDied;
    }

    public void InvokeOnceOnInit()
    {
        OnPropertyChanged(nameof(CurrentHp));
        OnPropertyChanged(nameof(MaxHP));
        OnPropertyChanged(nameof(IsDead));
    }

    private void HandleHpChanged(string changedType)
    {
        switch (changedType)
        {
            case nameof(CurrentHp):
                OnPropertyChanged(nameof(CurrentHp));
                break;
            case nameof(MaxHP):
                OnPropertyChanged(nameof(MaxHP));
                break;
        }
    }

    private void HandleDied(string changedType)
    {
        if (changedType == nameof(IsDead))
        {
            OnPropertyChanged(nameof(IsDead));
        }
    }

    public float CurrentHp => _monsterModel.CurrentHp;
    public float MaxHP => _monsterModel.MaxHP;
    public bool IsDead => _monsterModel.IsDead;
}
