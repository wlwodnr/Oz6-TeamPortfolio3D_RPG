using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    // 현재 진행중인 퀘스트목록
    private Dictionary<string, QuestModel> _activeQuests = new Dictionary<string, QuestModel>();
    // 완료했던 퀘스트 기록
    private HashSet<string> _completedQuestIds = new HashSet<string>();

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

        if (data == null)
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

    public void NotifyEnemyKilled(string enemyDataId)
    {
        if(string.IsNullOrEmpty(enemyDataId))
        {
            Debug.LogWarning("처치된 Enemy의 DataId가 비어 있어 퀘스트 진행도를 갱신할 수 없습니다.");
            return;
        }

        UpdateProgress("Kill", enemyDataId, 1);
    }

    public void UpdateProgress(string questType, string targetId, int amount)
    {
        if(string.IsNullOrEmpty(targetId))
        {
            Debug.Log("QuestManager: 퀘스트 진행도 갱신에 사용할 TargetId가 비어 있습니다.");

            return;
        }
        if(amount <= 0)
        {
            Debug.Log($"QuestManager: 퀘스트 진행도 증가량이 유효하지 않습니다.TargetId: {targetId}, Amount: {amount}");
            return;
        }

        if(GameDataManager.Instance == null)
        {
            Debug.LogWarning("QuestManager: GameDataManager가 없어 퀘스트 진행도를 갱신할 수 없습니다.");

            return;
        }

        foreach(QuestModel model in _activeQuests.Values)
        {
            if(model == null)
            {
                continue;
            }

            if(model.IsAccepted == false || model.IsCompleted || model.IsRewardReceived)
            {
                continue;
            }

            QuestData data = GameDataManager.Instance.GetQuestData(model.QuestDataId);

            if(data == null)
            {
                Debug.Log($"QuestManager: QuestData를 찾을 수 없습니다. QuestId: {model.QuestDataId}");
                continue;
            }
            if(string.IsNullOrEmpty(questType) == false && data.QuestType != questType)
            {
                continue ;
            }

            if(data.TargetId != targetId)
            {
                continue;
            }

            model.CurrentCount += amount;

            if(model.CurrentCount >= data.TargetCount)
            {
                model.CurrentCount = data.TargetCount;
                model.IsCompleted = true;
                Debug.Log($"QuestManager: 퀘스트 조건을 달성했습니다: {model.QuestDataId}");

            }
            Debug.Log($"QuestManager: 퀘스트 진행도 갱신: {model.QuestDataId} ({model.CurrentCount}/{data.TargetCount})");
            if (OnQuestStateChanged != null)
            {
                OnQuestStateChanged?.Invoke(model.QuestDataId);
            }
        }

    }


    //public void UpdateProgress(string targetId, int amount)
    //{
    //    foreach (QuestModel model in _activeQuests.Values)
    //    {
    //        QuestData data = GameDataManager.Instance.GetQuestData(model.QuestDataId);
    //        if (data == null)
    //            continue;

    //        // 퀘스트가 진행중이고 퀘스트 목표 ID가 일치할때 카운트
    //        if (data.TargetId == targetId && !model.IsCompleted)
    //        {
    //            model.CurrentCount += amount;
    //            Debug.Log($"퀘스트 진행도 갱신: {model.QuestDataId} ({model.CurrentCount}/{data.TargetCount})");

    //            // 퀘 목표 달성 시 완료 상태 true
    //            if(model.CurrentCount >= data.TargetCount)
    //            {
    //                model.CurrentCount = data.TargetCount;
    //                model.IsCompleted = true;
    //                Debug.Log($"퀘스트 조건을 달성했습니다 : {model.QuestDataId}");
    //            }

    //            // Ui 갱신 트리거
    //            if(OnQuestStateChanged != null)
    //            {
    //                OnQuestStateChanged.Invoke(model.QuestDataId);
    //            }
    //        }
    //    }
    //}

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
        _completedQuestIds.Add(questId);
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

    public string GetCompletedQuestId(string questId)
    {
        string completedId = questId;

        while (_completedQuestIds.Contains(completedId))
        {
            QuestData data = GameDataManager.Instance.GetQuestData(completedId);

            if(data == null || string.IsNullOrEmpty(data.NextQuestId))
            {
                break;
            }

            completedId = data.NextQuestId;
        }

        return completedId;
    }

    [ContextMenu("테스트용 Quest_001 킬 3 증가")]
    private void Test_KillMonster001()
    {
        UpdateProgress("Kill", "mob_goblin_1", 3);
    }
}
