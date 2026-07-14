using System;
using UnityEngine;

public class PlayerInfo
{
    public event Action<string> OnInfoChanged;

    private string _name;
    private int _curLevel;
    private float _totalExp;
    private int _skillPoint;
    private float _curHp;
    private float _curMp;
    private int _coins;
    private float _maxHp;
    private float _maxMp;
    private float _atkDamage;
    private float _atkSpeed;

    public string Name
    {
        get => _name;
        set { _name = value; OnInfoChanged?.Invoke(nameof(Name)); }
    }

    public int CurLevel
    {
        get => _curLevel;
        set { _curLevel = value; OnInfoChanged?.Invoke(nameof(CurLevel)); }
    }

    public float CurHp
    {
        get => _curHp;
        set { _curHp = value; OnInfoChanged?.Invoke(nameof(CurHp)); }
    }

    public float CurMp
    {
        get => _curMp;
        set { _curMp = value; OnInfoChanged?.Invoke(nameof(CurMp)); }
    }
    public float TotalExp
    {
        get => _totalExp;
        set { _totalExp = value; OnInfoChanged?.Invoke(nameof(TotalExp)); }
    }
    public int SkillPoint
    {
        get => _skillPoint;
        set { _skillPoint = value; OnInfoChanged?.Invoke(nameof(SkillPoint)); }
    }
    public int Coins
    {
        get => _coins;
        set { _coins = value; OnInfoChanged?.Invoke(nameof(Coins)); }
    }

    public float MaxHP
    {
        get => _maxHp;
        set { _maxHp = value; OnInfoChanged?.Invoke(nameof(MaxHP)); }
    }
    public float MaxMP
    {
        get => _maxMp;
        set { _maxMp = value; OnInfoChanged?.Invoke(nameof(MaxMP)); }
    }

    public float AtkDamage
    {
        get => _atkDamage;
        set { _atkDamage = value; OnInfoChanged?.Invoke(nameof(AtkDamage)); }
    }
    public float AtkSpeed
    {
        get => _atkSpeed;
        set { _atkSpeed = value; OnInfoChanged?.Invoke(nameof(AtkSpeed)); }
    }
}