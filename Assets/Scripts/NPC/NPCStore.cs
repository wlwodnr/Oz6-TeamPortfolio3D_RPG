using UnityEngine;

public class NPCStore : MonoBehaviour, IInteractable
{
    [SerializeField] private string NpcId;

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
