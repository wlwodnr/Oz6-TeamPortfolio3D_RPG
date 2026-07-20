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
        if (string.IsNullOrEmpty(NpcId))
        {
            Debug.LogWarning("NpcId가 없어서 상점을 열수없습니다");
            return;
        }

        if(StoreManager.Instance == null)
        {
            Debug.LogWarning("StoreManaager가 없어서 상점을 열수없습니다");
            return;
        }

        StoreManager.Instance.OpenStore(NpcId);
    }
}
