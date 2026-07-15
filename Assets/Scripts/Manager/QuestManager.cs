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

        // 생성 및 초기화
        QuestModel newQuestModel = new QuestModel();
        newQuestModel.QuestDataId = questId;
        newQuestModel.CurrentCount = 0;
        newQuestModel.IsAccepted = true;
        newQuestModel.IsCompleted = false;
        newQuestModel.IsRewardReceived = false;

        _activeQuests.Add(questId, newQuestModel);
        Debug.Log($"퀘스트를 수락했습니다 : {questId}");

        // UI에 알림
        if (OnQuestStateChanged != null)
        {
            OnQuestStateChanged.Invoke(questId);
        }
    }

    public void UpdateProgress(string targetId, int amount)
    {
        foreach (QuestModel model in _activeQuests.Values)
        {
            QuestData data = GameDataManager.Instance.GetQuestData(model.QuestDataId);
            if (data == null)
                continue;

            // 퀘스트가 진행중이고 퀘스트 목표 ID가 일치할때 카운트
            if (data.TargetId == targetId && !model.IsCompleted)
            {
                model.CurrentCount += amount;
                Debug.Log($"퀘스트 진행도 갱신: {model.QuestDataId} ({model.CurrentCount}/{data.TargetCount})");

                // 퀘 목표 달성 시 완료 상태 true
                if(model.CurrentCount >= data.TargetCount)
                {
                    model.CurrentCount = data.TargetCount;
                    model.IsCompleted = true;
                    Debug.Log($"퀘스트 조건을 달성했습니다 : {model.QuestDataId}");
                }

                // Ui 갱신 트리거
                if(OnQuestStateChanged != null)
                {
                    OnQuestStateChanged.Invoke(model.QuestDataId);
                }
            }
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

        QuestData data = GameDataManager.Instance.GetQuestData(questId);
        if (data == null)
            return;

        model.IsRewardReceived = true;
        Debug.Log($"보상 지급 요청: EXP: {data.RewardExp}, 골드: {data.RewardGold}");
        // 나중에 경험치는 경험치관리 매니저, 아이템은 아이템관리 매니너, 스킬포인트는 스킬포인트관리하는 매니저에게 요청
        // 예시) GameManager.Instance.AddExp(data.RewardExp); / SkillManager.Instance.AddSkillPoint(data.RewardSkillPoint);


        _activeQuests.Remove(questId);
        Debug.Log($"퀘스트 클리어! 현재 퀘스트 목록에서 제거됩니다 : {questId}");

        if(OnQuestStateChanged != null)
        {
            OnQuestStateChanged.Invoke(questId);
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
