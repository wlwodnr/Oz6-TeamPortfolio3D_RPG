using System;
using System.Collections.Generic;

[System.Serializable]
public class QuestData : GameDataBase
{
    public string QuestType;
    public string Description;
    public string TargetId;
    public int TargetCount;
}
