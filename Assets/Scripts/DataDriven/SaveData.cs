using System;
using System.Collections.Generic;


// 주석처리된 부분은 제가 임의로 틀만 만들어둔곳입니다 각 파트 담당하시는분이 편집해주세요
[Serializable]
public class SaveData  
{
    public PlayerSaveData PlayerData;
    public InventoryData Inventory;
    //public SkillData Skill;  플레이어 스킬 저장 데이터
    //public SpawnData Spawn; 플레이어 위치 저장 데이터
    //public QuestSaveData Quest;  퀘스트 저장 데이터
    //public DungeonData Dungeon; 던전관련 저장 데이터
}

[Serializable]
public class PlayerSaveData
{
    public PlayerInfo PlayerInfo;
}

//[Serializable]
//public class InventoryData
//{
//    public int Coins;
//    public List<ItemSlot> Slots = new List<ItemSlot>();
//}

[Serializable]
public class ItemSlot
{
    public string ItemId;
    public int Count;
}

[Serializable]
public class SkillData
{
    public List<string> LearnedSkills = new List<string>();  
}

[Serializable] 
public class QuestSaveData
{
    public List<string> CompleteQuests = new List<string>();
    public List<ProgressQuest> ActiveQuests = new List<ProgressQuest>();
    public List<string> InCompleteQuests = new List<string>();
}

[Serializable]
public class ProgressQuest
{
    public string QuestId;
    public bool IsCompleted;
    public List<ObjectiveData> Objectives;
}

[Serializable]
public class ObjectiveData
{
    public string ObjectiveId;
    public int CurrentCount;
    public int TargetCount;
    public bool IsCompleted;
}

[Serializable]
public class DungeonData
{
    public int ClearedFloor;
}

[Serializable]
public class SpawnData
{
    public float PlayerPosX;
    public float PlayerPosY;
    public float PlayerPosZ;
}