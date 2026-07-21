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

    public MonsterModel(float maxHp)
    {
        MaxHP = maxHp;
        _currentHp = maxHp;
        _isDead = false;
    }

    public void TakeDamage(float damage)
    {
        if (_isDead) return;

        _currentHp = Mathf.Clamp(_currentHp - damage, 0f, MaxHP);

        OnHpChanged?.Invoke(nameof(CurrentHp));

        if (_currentHp <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (_isDead) return;

        _isDead = true;
        OnDied?.Invoke(nameof(IsDead));
    }
}