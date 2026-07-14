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
    public int RewardSkillPoint;
    public string AcceptGroupId;
    public string RepeatGroupId;
    public string ClearGroupId;
    public string NextQuestId;
}
