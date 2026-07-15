using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : UIBase
{
    [SerializeField] private GameObject Layout_characterName;
    [SerializeField] private Text Text_character;
    [SerializeField] private Text Text_description;
    [SerializeField] private UIButton Btn_next;

    private string _currentDialogueId;
    private Queue<string> _descriptionQueue = new Queue<string>();

    private string[] _dialogueIds;
    private int _currentDialogueIndex;

    private void Awake()
    {
        Btn_next.BindOnClickButtonEvent(OnClick_Next);
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
            UIManager.Instance.CloseContentUI(UIType.DialogueUI);
        }
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
    

    public void StartDialogue(string dialogueId)
    {
        DialogueData dialogueData = GameDataManager.Instance.GetDialogueData(dialogueId);

        if (dialogueData == null)
        {
            Debug.LogWarning($"다이얼로그 데이터가 존재하지 않습니다 DialogueId : {dialogueId}");
            return;
        }
        
        _currentDialogueId = dialogueId;
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
            var characterData = GameDataManager.Instance.GetCharacterData(characterDataId);
            if (characterData != null)
            {
                Text_character.text = characterData.Name;
            }
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
}
