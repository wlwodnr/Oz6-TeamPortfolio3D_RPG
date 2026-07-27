using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : UIBase
{
    [SerializeField] private GameObject Layout_characterName;
    [SerializeField] private Text Text_character;
    [SerializeField] private Text Text_description;
    [SerializeField] private UIButton Btn_next;
    [SerializeField] private UIButton Btn_exit;
    [SerializeField] private UIButton Btn_accept;

    private Queue<string> _descriptionQueue = new Queue<string>();

    private string[] _dialogueIds;
    private int _currentDialogueIndex;

    private string _currentQuestId;
    private bool _isQuestAcceptDialogue;
    private bool _isQuestClearDialogue;

    private void OnEnable()
    {
        Btn_next.BindOnClickButtonEvent(OnClick_Next);
        Btn_accept.BindOnClickButtonEvent(OnClick_Accept, true);
        Btn_exit.BindOnClickButtonEvent(OnClick_Exit);
    }

    private void OnDisable()
    {
        Btn_next.UnBindAllOnClickButtonEvent();
        Btn_accept.UnBindAllOnClickButtonEvent();
        Btn_exit.UnBindAllOnClickButtonEvent();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnClick_Next();
        }
    }

    public void OnClick_Next()
    {
        bool isNextDescriptionExist = CheckAndSetDescription();

        if (isNextDescriptionExist)
        {
            return;
        }
        
        bool isNextDialogueExist = TryStartNextDialogue();

        if (isNextDialogueExist == false)
        {
            if (_isQuestAcceptDialogue)
            {
                Btn_next.gameObject.SetActive(false);
                Btn_accept.gameObject.SetActive(true);
                return;
            }

            if (_isQuestClearDialogue)
            {
                HandleQuestClear();
                return;
            }

            UIManager.Instance.CloseContentUI(UIType.DialogueUI);
        }
    }

    public void OnClick_Accept()
    {
        if (string.IsNullOrEmpty(_currentQuestId))
        {
            Debug.LogWarning("수락할 퀘스트 Id가 없습니다");
            return;
        }

        if (QuestManager.Instance == null)
        {
            Debug.LogWarning("QuestManager가 존재하지 않습니다");
            return;
        }

        QuestManager.Instance.AcceptQuest(_currentQuestId);
        UIManager.Instance.CloseContentUI(UIType.DialogueUI);
    }

    public void OnClick_Exit()
    {
        Debug.Log("대화 닫기 버튼을 눌렀습니다");
        UIManager.Instance.CloseContentUI(UIType.DialogueUI);
    }

    private bool CheckAndSetDescription()
    {
        bool isNextDescriptionExsist = (_descriptionQueue.Count > 0);
        if (isNextDescriptionExsist)
        {
            string desc = _descriptionQueue.Dequeue();
            SetCurrentDialogueDescription(desc);
        }

        return isNextDescriptionExsist;
    }

    public void OpenDialogueGroup(string groupId)
    {
        _currentQuestId = null;
        _isQuestAcceptDialogue = false;
        _isQuestClearDialogue = false;

        Btn_next.gameObject.SetActive(true);
        Btn_accept.gameObject.SetActive(false);

        StartDialogueGroup(groupId);
    }

    private void StartDialogueGroup(string groupId)
    {
        DialogueGroupData dialogueGroupData = GameDataManager.Instance.GetDialogueGroupData(groupId);

        if (dialogueGroupData == null)
        {
            Debug.LogWarning($"다이얼로그 데이터가 존재하지 않습니다 {dialogueGroupData}");
            return;
        }
        
        if (string.IsNullOrEmpty(dialogueGroupData.DialogueIdList))
        {
            Debug.LogWarning($"다이얼로그 그룹에 대사가 없습니다 GroupId : {groupId}");
            return;
        }

        _dialogueIds = dialogueGroupData.DialogueIdList.Split(',');

        for (int i = 0; i < _dialogueIds.Length; i++)
        {
            _dialogueIds[i] = _dialogueIds[i].Trim();
        }

        _currentDialogueIndex = 0;
        StartDialogue(_dialogueIds[_currentDialogueIndex]);
    }
    
    public void OpenQuestAcceptDialogue(string questId)
    {
        QuestData questData = GameDataManager.Instance.GetQuestData(questId);

        if (questData == null)
        {
            Debug.LogWarning($"퀘스트 데이터가 존재하지 않습니다 Questid: {questId}");
            return;
        }

        if (string.IsNullOrEmpty(questData.AcceptGroupId))
        {
            Debug.LogWarning($"퀘스트 수주 대화그룹이 없습니다 QuestId: {questId}");
            return;
        }

        _currentQuestId = questId;
        _isQuestAcceptDialogue = true;
        _isQuestClearDialogue = false;

        Btn_next.gameObject.SetActive(true);
        Btn_accept.gameObject.SetActive(false);

        StartDialogueGroup(questData.AcceptGroupId);
    }

    public void OpenQuestDialogue(string questId)
    {
        if (QuestManager.Instance != null)
        {
            questId = QuestManager.Instance.GetCompletedQuestId(questId);
        }

        QuestData questData = GameDataManager.Instance.GetQuestData(questId);

        if (questData == null)
        {
            Debug.LogWarning($"퀘스트 데이터가 존재하지 않습니다");
            return;
        }

        QuestModel model = null;

        if (QuestManager.Instance != null)
        {
            model = QuestManager.Instance.GetActiveQuestModel(questId);
        }

        if (model == null)
        {
            OpenQuestAcceptDialogue(questId);
            return;
        }

        if (model.IsCompleted)
        {
            OpenQuestClearDialogue(questId);
            return;
        }

        OpenQuestRepeatDialogue(questId);
    }

    public void StartDialogue(string dialogueId)
    {
        DialogueData dialogueData = GameDataManager.Instance.GetDialogueData(dialogueId);

        if (dialogueData == null)
        {
            Debug.LogWarning($"다이얼로그 데이터가 존재하지 않습니다 DialogueId : {dialogueId}");
            return;
        }
        
        _descriptionQueue.Clear();
        
        if (dialogueData.Description.Contains("<np>"))
        {
            string[] dialogueDescriptionList = dialogueData.Description.Split("<np>");
            foreach (string desc in dialogueDescriptionList)
            {
                _descriptionQueue.Enqueue(desc);
            }
            CheckAndSetDescription();
        }
        else
        {
            SetCurrentDialogueDescription(dialogueData.Description);
        }

        SetCharacterName(dialogueData.CharacterDataId);
    }

    private void SetCharacterName(string characterDataId)
    {
        bool isActive = (string.IsNullOrEmpty(characterDataId) == false);
        Layout_characterName.SetActive(isActive);

        if (isActive)
        {
            Text_character.text = characterDataId;
        }
    }

    private void SetCurrentDialogueDescription(string description)
    {
        Text_description.text = description;
    }

    private bool TryStartNextDialogue()
    {
        if (_dialogueIds == null || _dialogueIds.Length == 0)
        {
            return false;
        }

        if (_currentDialogueIndex >= _dialogueIds.Length - 1)
        {
            return false;
        }

        _currentDialogueIndex++;
        StartDialogue(_dialogueIds[_currentDialogueIndex]);

        return true;
    }

    public void OpenQuestRepeatDialogue(string questId)
    {
        QuestData questData = GameDataManager.Instance.GetQuestData(questId);
        if (questData == null || string.IsNullOrEmpty(questData.RepeatGroupId))
        {
            Debug.LogWarning($"반복 대화 그룹이 없습니다");
            return;
        }

        _currentQuestId = questId;
        _isQuestAcceptDialogue = false;
        _isQuestClearDialogue = false;

        Btn_next.gameObject.SetActive(true);
        Btn_accept.gameObject.SetActive(false);

        StartDialogueGroup(questData.RepeatGroupId);
    }

    public void OpenQuestClearDialogue(string questId)
    {
        QuestData questData = GameDataManager.Instance.GetQuestData(questId);
        if (questData == null || string.IsNullOrEmpty(questData.ClearGroupId))
        {
            Debug.LogWarning("클리어 대화 그룹이 없습니다");
            return;
        }

        _currentQuestId = questId;
        _isQuestAcceptDialogue = false;
        _isQuestClearDialogue = true;

        Btn_next.gameObject.SetActive(true);
        Btn_accept.gameObject.SetActive(false);

        StartDialogueGroup(questData.ClearGroupId);
    }

    private void HandleQuestClear()
    {
        if (string.IsNullOrEmpty(_currentQuestId))
        {
            Debug.LogWarning("완료 처리할 퀘스트 id가 없습니다");
            UIManager.Instance.CloseContentUI(UIType.DialogueUI);
            return;
        }

        if(QuestManager.Instance == null)
        {
            Debug.LogWarning("QuestManager가 없습니다");
            UIManager.Instance.CloseContentUI(UIType.DialogueUI);
            return;
        }

        string clearedQuestId = _currentQuestId;
        
        QuestManager.Instance.CompleteQuest(clearedQuestId);
        _isQuestClearDialogue = false;

        UIManager.Instance.CloseContentUI(UIType.DialogueUI);
    }
}
