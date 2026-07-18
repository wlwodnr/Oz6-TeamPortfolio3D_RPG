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

    public void Interact()
    {

    }
}
