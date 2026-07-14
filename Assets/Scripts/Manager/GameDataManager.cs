using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            LoadAll();
        }
        else
        {
            Debug.Log("GameDataManager가 중복되어 파괴합니다");
            Destroy(gameObject);
        }
    }

    [Serializable]
    private class SerializationWrapper<T>
    {
        public List<T> items;
    }

    public Dictionary<string, QuestData> QuestDataList { get; private set; } = new Dictionary<string, QuestData>();
    public Dictionary<string, SkillData> SkillDataList { get; private set; } = new Dictionary<string, SkillData>();
    public Dictionary<string, CharacterData> CharacterDataList { get; private set; } = new Dictionary<string, CharacterData>();
    public Dictionary<string, PlayerStatData> PlayerStatDataList { get; private set; } = new Dictionary<string, PlayerStatData>();
    public Dictionary<string, ItemData> ItemDataList { get; private set; } = new Dictionary<string, ItemData>();

    private Dictionary<string, T> LoadData<T>(string tableName) where T : GameDataBase
    {
        string resourcePath = $"JsonOutput/{tableName}";

        TextAsset textAsset = Resources.Load<TextAsset>(resourcePath);

        if(textAsset == null)
        {
            Debug.LogError($"[Error] 리소스를 찾을 수 없습니다: Resources/{resourcePath}");
            return new Dictionary<string, T>();
        }

        try
        {
            string jsonString = textAsset.text;

            string wrappedJson = "{\"items\":" + jsonString + "}";
            SerializationWrapper<T> wrapper = JsonUtility.FromJson<SerializationWrapper<T>>(wrappedJson);

            if(wrapper != null && wrapper.items != null)
            {
                Debug.Log($"{typeof(T).Name} 데이터를 {wrapper.items.Count}개 로드했습니다");
                return wrapper.items.ToDictionary(item => item.Id.ToString());
            }
        }
        catch(Exception ex)
        {
            Debug.LogError($"[{typeof(T).Name} Json 로드 오류] {ex.Message}");
        }

        return new Dictionary<string, T>();
    }


    public void LoadAll()
    {

        QuestDataList = LoadData<QuestData>("QuestData");
    }


    public QuestData GetQuestData(string id)
    {
        if(QuestDataList == null || string.IsNullOrEmpty(id)) return null;

        return QuestDataList.TryGetValue(id, out var item) ? item : null;
    }

    public SkillData GetSkill(string id)
    {
        if (SkillDataList == null || string.IsNullOrEmpty(id)) return null;

        return SkillDataList.TryGetValue(id, out var item) ? item : null;
    }

    public CharacterData GetCharacterData(string id)
    {
        if (CharacterDataList == null || string.IsNullOrEmpty(id)) return null;

        return CharacterDataList.TryGetValue(id, out var item) ? item : null;
    }

    public PlayerStatData GetPlayerStatData(string id)
    {
        if (PlayerStatDataList == null || string.IsNullOrEmpty(id)) return null;

        return PlayerStatDataList.TryGetValue(id, out var item) ? item : null;
    }
    public ItemData GetItemData(string id)
    {
        if (ItemDataList == null || string.IsNullOrEmpty(id)) return null;

        return ItemDataList.TryGetValue(id, out var item) ? item : null;
    }
}
