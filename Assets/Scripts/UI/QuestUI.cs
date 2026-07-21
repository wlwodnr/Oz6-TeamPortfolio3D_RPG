using UnityEngine;
using UnityEngine.UI;

public class QuestUI : UIBase
{
    [SerializeField] private Text Text_QuestTitle;
    [SerializeField] private Text Text_QuestDescription;
    [SerializeField] private Text Text_Progress;

    private void OnEnable()
    {
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestStateChanged += OnQuestStateChanged;
    }

    private void OnDisable()
    {
        if(QuestManager.Instance != null)
            QuestManager.Instance.OnQuestStateChanged -= OnQuestStateChanged;
    }

    private void OnQuestStateChanged(string questId)
    {

    }

    private void Show()
    {

    }

    private void Hide()
    {

    }
}
