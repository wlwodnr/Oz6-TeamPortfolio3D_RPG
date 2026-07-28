using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NetworkSaveService
{


    private string GetPath()
    {
        return Path.Combine(Application.persistentDataPath, "PlayerSaveData.json");
    }


    public SaveData CreateSaveData()
    {
        var createdSaveData = new SaveData();

        var PlayerModel = NetworkManager.Inst.LocalPlayerService.GetLocalPlayerModel();
        var InventoryModel = NetworkManager.Inst.InventoryService.GetLocalPlayerInventoryModel();

        createdSaveData.PlayerData = PlayerModel.CaptureData();
        createdSaveData.Inventory = InventoryModel.CaptureInventoryData();
        createdSaveData.Quest = QuestManager.Instance.CaptureQuestData();
        createdSaveData.Skill = PlayerModel.CaptureSkillData();
        createdSaveData.Treasure.OpenedId = GameObjectManager.Instance.CaptureTreasureIdData();


        return createdSaveData;
    }

    public void RequestSaveData()
    {
        var createdSaveModel = CreateSaveData();

        string json = JsonUtility.ToJson(createdSaveModel, true);
        File.WriteAllText(GetPath(), json); 
        Debug.Log($"저장 완료: {GetPath()}");
    }

    public void RequestLoadSaveData()
    {
        string path = GetPath();
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("데이터를 불러왔습니다.");
            SetAllModelOnLoad(data);
        }
        else
        {
            Debug.LogWarning("세이브 파일이 없습니다. 새 데이터를 생성합니다.");
            RequestSaveData();
            RequestLoadSaveData();
        }
    }


    public void SetAllModelOnLoad(SaveData saveData)
    {
        if (saveData == null) return;

        NetworkManager.Inst.LocalPlayerService.LoadData(saveData);
        QuestManager.Instance.LoadQuestData(saveData.Quest);
        NetworkManager.Inst.InventoryService.LoadInventoryData(saveData.Inventory);
        GameObjectManager.Instance.LoadOpenedTreasureId(saveData.Treasure.OpenedId);
    }

}