using System;
using UnityEngine;

public class MonsterModel
{
    public event Action<string> OnHpChanged;
    public event Action<string> OnDied;

    public float MaxHP { get; private set; }

    private float _currentHp;
    public float CurrentHp
    {
        get { return _currentHp; }
        private set { _currentHp = value; }
    }

    private bool _isDead;
    public bool IsDead
    {
        get { return _isDead; }
        private set { _isDead = value; }
    }

    private EnemyStatus _targetStatus;

    public MonsterModel(EnemyStatus targetStatus)
    {
        _targetStatus = targetStatus;

        if (_targetStatus != null)
        {
            MaxHP = _targetStatus.MaxHp;
            _currentHp = _targetStatus.CurrentHp;
            _isDead = _targetStatus.IsDead;
        }
    }

    public void UpdateStatusFromEntity()
    {
        if (_targetStatus == null) return;

        if (_currentHp != _targetStatus.CurrentHp)
        {
            _currentHp = _targetStatus.CurrentHp;
            OnHpChanged?.Invoke(nameof(CurrentHp));
        }

        if (_isDead != _targetStatus.IsDead)
        {
            _isDead = _targetStatus.IsDead;
            if (_isDead)
            {
                OnDied?.Invoke(nameof(IsDead)); 
            }
        }
    }
}