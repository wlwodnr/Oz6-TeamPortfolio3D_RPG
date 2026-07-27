using System;
using UnityEngine;

public class PlayerInfo
{
    public event Action<string> OnInfoChanged;

    private string _name;
    private int _curLevel = 1;
    private float _totalExp;
    private int _skillPoint;
    private float _curHp;
    private float _curMp;
    private int _coins;
    
    public string Name
    {
        get => _name;
        set { _name = value; OnInfoChanged?.Invoke(nameof(Name)); }
    }

    public int CurLevel
    {
        get => _curLevel;
        set
        {
            int nextLevel = Mathf.Max(1, value);

            if (_curLevel == nextLevel)
            {
                return;
            }

            _curLevel = nextLevel;
            OnInfoChanged?.Invoke(nameof(CurLevel));
        }
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
        set
        {
            _totalExp = float.IsNaN(value) || float.IsInfinity(value) ? 0f : Mathf.Max(0f, value);
            int calculatedLevel = PlayerLevelProgression.CalculateLevel(_totalExp);

            if (_curLevel != calculatedLevel)
            {
                CurLevel = calculatedLevel;
            }

            OnInfoChanged?.Invoke(nameof(TotalExp));
        }
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
}
