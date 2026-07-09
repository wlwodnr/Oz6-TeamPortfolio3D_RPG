using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemModel
{
    public long ItemUniqueId;
    public string ItemDataId;
    public int ItemStackCount;
}

[Serializable]
public class PlayerModel
{
    public string PlayerName;
    public int PlayerTotalExp;
    public int PlayerLevel;
    public int CurrentHP;
    public int CurrentMP;
    public int CrrentAtk;
    public int UnusedSkillPoint;

    public string LastMapDataId;
    public Vector3 LastMapPosition;

    public List<ItemModel> ItemList = new List<ItemModel>();
    public List<ItemModel> EquitpmentList = new List<ItemModel>();
}
