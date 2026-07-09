using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    // 현재 진행중인 퀘스트목록
    private Dictionary<string, QuestModel> _activeQuests = new Dictionary<string, QuestModel>();

    // MVVM UI 연동 이벤트 작성할 예정

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

    }

    public void CompleteQuest(string questId)
    {

    }
}
