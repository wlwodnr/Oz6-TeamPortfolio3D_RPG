using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    // 현재 진행중인 퀘스트목록
    private Dictionary<string, QuestModel> _activeQuests = new Dictionary<string, QuestModel>();

    public delegate void QuestStateChangedHandler(string questId);
    public event QuestStateChangedHandler OnQuestStateChanged;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("QuestManager가 중복되어 파괴합니다");
            Destroy(gameObject);
        }
    }

    public void AcceptQuest(string questId)
    {
        if (_activeQuests.ContainsKey(questId))
        {
            Debug.LogWarning($"이미 수락한 퀘스트입니다 quest ID: {questId}");
            return;
        }

        QuestData data = GameDataManager.Instance.GetQuestData(questId);

        if(data == null)
        {
            Debug.LogWarning($"QuestData를 찾을수 없습니다 QuestId : {questId}");
            return;
        }
    }

    public void CompleteQuest(string questId)
    {
        if (!_activeQuests.TryGetValue(questId, out var model))
        {
            Debug.LogWarning($"진행중인 퀘스트 목록에 없습니다 QUestId: {questId}");
            return;
        }

        if (!model.IsCompleted)
        {
            Debug.LogWarning($"아직 퀘스트 조건을 만족하지 못했습니다 QuestId: {questId}");
            return;
        }

        if (model.IsRewardReceived)
        {
            Debug.LogWarning($"이미 보상이 지급된 퀘스트입니다 QuestId: {questId}");
            return;
        }
    }

    public QuestModel GetActiveQuestModel(string questId)
    {
        if (_activeQuests.TryGetValue(questId, out var model))
        {
            return model;
        }

        return null;
    }
}
