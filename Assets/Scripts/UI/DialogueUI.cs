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
        
        bool isNextDialogueExist = CheckAndStartNextDialogue();
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

    private bool CheckAndStartNextDialogue()
    {
        var dialogueData = GameDataManager.Instance.GetDialogueData(_currentDialogueId);
        if (dialogueData == null)
        {
            Debug.LogWarning($"다이얼로그 데이터가 존재하지 않습니다 {dialogueData}");
            return false;
        }
        
        string nextDialogueId = dialogueData.NextDialogueId;
        if (string.IsNullOrEmpty(nextDialogueId) == false)
        {
            StartDialogue(nextDialogueId);
            return true;
        }

        return false;
    }
    

    public void StartDialogue(string dialogueId)
    {
        var dialogueData = GameDataManager.Instance.GetDialogueData(dialogueId);
        if (dialogueData == null)
        {
            Debug.LogWarning($"다이얼로그 데이터가 존재하지 않습니다 {dialogueData}");
            return;
        }
        
        _currentDialogueId = dialogueId;
        
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
}
