using System;

[System.Serializable]
public class QuestModel
{
    public string QuestDataId;
    public int CurrentCount;
    public bool IsAccepted;
    public bool IsCompleted;
    public bool IsRewardReceived;

    public QuestModel(ProgressQuest data)
    {
        QuestDataId = data.QuestDataId;
        CurrentCount = data.CurrentCount;
        IsAccepted = data.IsAccepted;
        IsCompleted = data.IsCompleted;
        IsRewardReceived = data.IsRewardReceived;
    }
}
