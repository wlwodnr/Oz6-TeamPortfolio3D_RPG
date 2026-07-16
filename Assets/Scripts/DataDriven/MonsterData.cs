using System;
using System.Collections.Generic;

[System.Serializable]
public class MonsterData : GameDataBase
{
    public string Name;
    public string Description;
    public string PrefabPath;

    public int BaseHp;
    public int BaseAttack;
    public float MoveSpeed;

    public float DetectRange;
    public float AttackRange;
    
    public float DropEXP;
    public float StopDistance;
}
