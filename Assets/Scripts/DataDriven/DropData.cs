using System;

[Serializable]
public class DropData : GameDataBase
{
    public string MonsterDataId;
    public string ItemDataId;
    public float DropChance;
    public int MinCount;
    public int MaxCount;
}
