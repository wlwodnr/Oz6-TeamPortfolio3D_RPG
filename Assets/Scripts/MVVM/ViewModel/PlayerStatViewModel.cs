using UnityEngine;

public class PlayerStatViewModel : ViewModelBase
{
    private PlayerModel _playerModel;

    public PlayerStatViewModel(PlayerModel playerModel)
    {
        _playerModel = playerModel;
        _playerModel.OnPlayerStatsChanged += HandleStatsChanged;
        _playerModel.OnPlayerInfoChanged += HandleInfoChanged;
    }

    public void InvokeOnceOnInit()
    {
        OnPropertyChanged(nameof(AttackPower));
        OnPropertyChanged(nameof(MaxHP));
        OnPropertyChanged(nameof(MaxMP));
        OnPropertyChanged(nameof(AttackSpeed));
        OnPropertyChanged(nameof(SkillPoint));
    }

    private void HandleStatsChanged(string changedType)
    {
        switch (changedType)
        {
            case nameof(StatType.AttackPower):
                OnPropertyChanged(nameof(AttackPower));
                break;
            case nameof(StatType.MaxHP):
                OnPropertyChanged(nameof(MaxHP));
                break;
            case nameof(StatType.MaxMP):
                OnPropertyChanged(nameof(MaxMP));
                break;
            case nameof(StatType.AttackSpeed):
                OnPropertyChanged(nameof(AttackSpeed));
                break;
        }
    }

    private void HandleInfoChanged(string changedType)
    {
        if (changedType == nameof(SkillPoint))
        {
            OnPropertyChanged(nameof(SkillPoint));
        }
    }

    public int SkillPoint
    {
        get => _playerModel.Info.SkillPoint;
        set { if (_playerModel.Info.SkillPoint != value) _playerModel.Info.SkillPoint = value; }
    }

    public float AttackPower => _playerModel.GetStatValue(StatType.AttackPower);
    public float MaxHP => _playerModel.GetStatValue(StatType.MaxHP);
    public float MaxMP => _playerModel.GetStatValue(StatType.MaxMP);
    public float AttackSpeed => _playerModel.GetStatValue(StatType.AttackSpeed);
}
