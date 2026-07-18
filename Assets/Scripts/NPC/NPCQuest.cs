using UnityEngine;

public class NPCQuest : MonoBehaviour, IInteractable
{
    [SerializeField] private string QuestId;

    public bool CanInteract
    {
        get
        {
            return true;
        }
    }

    // 테스트용이니까 나중엔 지우면 됩니다
    [ContextMenu("테스트용 NPC 상호작용 실행")]
    private void Test_Interact()
    {
        Interact();
    }

    public void Interact()
    {
        if (string.IsNullOrEmpty(QuestId))
        {
            Debug.LogWarning($"{gameObject.name} : QuestId가 없어서 상호작용을 할수없습니다");
            return;
        }

        if(UIManager.Instance == null)
        {
            Debug.LogWarning($"{gameObject.name} : UIManager이 없어서 다이얼로그 출력 불가");
            return;
        }

        DialogueUI dialogueUI = UIManager.Instance.OpenDialogueUI();

        if (dialogueUI == null)
        {
            Debug.LogWarning($"{gameObject.name} : DialogueUI가 없어서 대화를 시작할 수 없습니다");
            return;
        }

        dialogueUI.OpenQuestDialogue(QuestId);
    }
}
