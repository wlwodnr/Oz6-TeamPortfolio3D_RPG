using JetBrains.Annotations;
using System;
using System.Collections.Generic;


// 주석처리된 부분은 제가 임의로 틀만 만들어둔곳입니다 각 파트 담당하시는분이 편집해주세요
[Serializable]
public class SaveData  
{
    public PlayerSaveData PlayerData;
    public InventoryData Inventory;
    public SkillSaveData Skill;  
    public QuestSaveData Quest;
    public TreasureData Treasure;
}

[Serializable]
public class PlayerSaveData
{
    public string Name;
    public int CurLevel = 1;
    public float TotalExp;
    public int SkillPoint;
    public float CurHp;
    public float CurMp;
    public int Coins;
}

[Serializable]
public class TreasureData
{
    public List<string> OpenedId = new List<string>();
}

[Serializable]
public class ItemSlot
{
    public string ItemId;
    public int Count;
}

[Serializable]
public class SkillSaveData
{
    public List<string> LearnedActiveSkills = new List<string>();
    public List<string> LearnedPassiveSkills = new List<string>();
}

[Serializable] 
public class QuestSaveData
{
    public List<ProgressQuest> ActiveQuests = new List<ProgressQuest>();
    public List<string> CompletedQuestIds = new List<string>();
}

[Serializable]
public class ProgressQuest
{
    public string QuestDataId;
    public int CurrentCount;
    public bool IsAccepted;
    public bool IsCompleted;
    public bool IsRewardReceived;

    public ProgressQuest(QuestModel data)
    {
        QuestDataId = data.QuestDataId;
        CurrentCount = data.CurrentCount;
        IsAccepted = data.IsAccepted;
        IsCompleted = data.IsCompleted;
        IsRewardReceived = data.IsRewardReceived;
    }
}

[Serializable]
public class ObjectiveData
{
    public string ObjectiveId;
    public int CurrentCount;
    public int TargetCount;
    public bool IsCompleted;
}
