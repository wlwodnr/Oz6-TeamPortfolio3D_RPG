using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NetworkSaveService
{


    private string GetPath()
    {
        return Path.Combine(Application.persistentDataPath, "DaniTechSaveData.json");
    }


    public SaveData CreateSaveData()
    {
        var createdSaveData = new SaveData();

        var PlayerModel = NetworkManager.Inst.LocalPlayerService.GetLocalPlayerModel();
        var InventoryModel = NetworkManager.Inst.InventoryService.GetLocalPlayerInventoryModel();

        createdSaveData.PlayerData = PlayerModel.CaptureData();
        createdSaveData.Inventory = InventoryModel.CaptureInventoryData();




        return createdSaveData;
    }

    public void RequstSaveData()
    {
        var createdSaveModel = CreateSaveData();

        string json = JsonUtility.ToJson(createdSaveModel, true);
        File.WriteAllText(GetPath(), json); 
        Debug.Log($"저장 완료: {GetPath()}");
    }

    public void RequstLoadSaveData()
    {
        string path = GetPath();
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("데이터를 불러왔습니다.");
            SetAllViewModelOnLoad(data);
        }
        else
        {
            Debug.LogWarning("세이브 파일이 없습니다. 새 데이터를 생성합니다.");
            RequstSaveData();
            RequstLoadSaveData();
        }
    }


    public void SetAllViewModelOnLoad(SaveData saveData)
    {

    }

}