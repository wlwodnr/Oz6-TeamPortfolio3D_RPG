using UnityEngine;
using UnityEngine.UI;

public class QuestUI : UIBase
{
    [SerializeField] private Text Text_QuestTitle;
    [SerializeField] private Text Text_QuestDescription;
    [SerializeField] private Text Text_Progress;

    //연출용 테스트라고 보셔도 됩니다 
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject Effect_QuestComplete;

    private string _trackedQuestId;

    private void OnEnable()
    {
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestStateChanged += OnQuestStateChanged;

        RefreshFromCurrentState();
    }

    private void OnDisable()
    {
        if(QuestManager.Instance != null)
            QuestManager.Instance.OnQuestStateChanged -= OnQuestStateChanged;
    }

    private void OnQuestStateChanged(string questId)
    {
        RefreshFromCurrentState();
    }

    private void RefreshFromCurrentState()
    {
        if (QuestManager.Instance == null) return;

        if(QuestManager.Instance.TryGetDisplayQuest(out string questId, out QuestModel model) == false)
        {
            Hide();
            return;
        }

        QuestData questData = GameDataManager.Instance.GetQuestData(questId);
        if (questData == null)
        {
            Hide();
            return;
        }

        _trackedQuestId = questId;
        Show();

        Text_QuestTitle.text = questData.QuestTitle;
        Text_QuestDescription.text = questData.Description;
        Text_Progress.text = $"({model.CurrentCount} / {questData.TargetCount})";

        SetCompleteEffect(model.IsCompleted);
    }

    // 연출테스트용
    private void SetCompleteEffect(bool isComplete)
    {
        if(Effect_QuestComplete != null)
            Effect_QuestComplete.SetActive(isComplete);

        Color color = isComplete ? Color.yellow : Color.white;
        Text_QuestTitle.color = color;
        Text_QuestDescription.color = color;
        Text_Progress.color = color;

        //Text_Progress.color = isComplete ? Color.yellow : Color.white;
    }

    private void Show()
    {
        gameObject.SetActive(true);
        if(canvasGroup != null)
            canvasGroup.alpha = 1;
    }

    private void Hide()
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 0;
        else
            gameObject.SetActive(false);
    }
}
