using System;
using System.Collections.Generic;

[System.Serializable]
public class QuestData : GameDataBase
{
    public string QuestType;
    public string Description;
    public string TargetId;
    public int TargetCount;
    public int RewardExp;
    public int RewardGold;
    public List<string> RewardItemIdList;
    public string NextQuestId;
}
