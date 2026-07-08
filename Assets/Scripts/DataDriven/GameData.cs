using System;
using System.Collections.Generic;

[System.Serializable]
public class GameDataBase
{
    public string Id;
}

[System.Serializable]
public class PlayerStatData : GameDataBase
{
    public string Comment;
    public int Atk;
    public int HP;
    public int MP;
    public int AtkSpeed;
    public int SkillPoint;
}