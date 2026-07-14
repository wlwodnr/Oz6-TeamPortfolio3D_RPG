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

[System.Serializable]
public class ItemData : GameDataBase
{
    public string Name;
    public string Description;
    public string ItemType;
    public string Grade;
    public string MaxStackCount;
    public string SellingPrice;
    public string IconPath;
    public string UseItemType;
    //public List<string> UseItemParameterList;
}