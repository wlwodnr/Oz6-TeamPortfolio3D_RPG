using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Text Text_StackCount;
    [SerializeField] private UIButton Button_Slot;
    [SerializeField] private Image Image_Icon;
    [SerializeField] private Image Image_Frame;
    [SerializeField] private Image Image_Selected;

    private event Action<long> OnSelectEvent;

    public long SlotItemUniqueId { get; private set; }
    public bool IsUsableItem { get; private set; }

    private void OnEnable()
    {
        Image_Selected.gameObject.SetActive(false);
        Button_Slot.BindOnClickButtonEvent(OnClick_SelectItem);
    }

    private void OnDisable()
    {
        OnSelectEvent = null;

        if (Button_Slot != null)
        {
            Button_Slot.UnBindAllOnClickButtonEvent();
        }
    }

    public void BindSlotViewModel(SlotViewModel slotVm)
    {
        if (slotVm == null)
        {
            ClearSlot();
            return;
        }

        gameObject.SetActive(true);
        InitSlot(slotVm.GetSlotId(), slotVm.ItemId, slotVm.Count);
    }

    public void InitSlot(long slotUniqueId, string itemDataId, int itemStackCount)
    {
        SlotItemUniqueId = slotUniqueId;
        SetIcon(itemDataId, itemStackCount);
    }

    public void SetIcon(string itemDataId, int itemCount)
    {
        var itemData = GameDataManager.Instance.GetItemData(itemDataId);
        if (itemData == null)
        {
            Debug.LogWarning($"Item 데이터를 불러올 수 없습니다! 경로:{itemDataId}");
            return;
        }

        string iconPath = itemData.IconPath;
        if (string.IsNullOrEmpty(iconPath) == true)
        {
            Debug.LogWarning($"Item 데이터에 아이콘 경로가 존재하지 않습니다.");
            return;
        }

        IsUsableItem = (string.IsNullOrEmpty(itemData.UseItemType) == false);

        // + Addressable을 적용하면서 비동기로 바뀌었다
        //DaniTechResourceManager.Inst.LoadSprite(iconPath, (sprite) => {
        //    Image_Icon.sprite = sprite;
        //});

        //GameUtil.LoadAndSetSpriteImage(Image_Icon, iconPath).Forget();

        //var sprite = GameUtil.LoadSpriteCanBeNull(iconPath);
        //if(sprite == null)
        //{
        //    Debug.LogWarning($"Sprite를 불러올 수 없습니다! 경로:{iconPath}");
        //    return;
        //}
        //Image_Icon.sprite = sprite;

        Text_StackCount.text = $"{itemCount}";
    }

    public void OnClick_SelectItem()
    {
        OnSelectEvent?.Invoke(SlotItemUniqueId);

        Debug.Log($"{SlotItemUniqueId} 슬롯 클릭됨");
    }

    public void BindSlotSelectEvent(Action<long> onSelectEvent)
    {
        OnSelectEvent = onSelectEvent;
    }

    // 수동태로 자신의 상태UI를 변경
    public bool ChangeSelectedState(long selectedItemUniqueId)
    {
        bool isSelected = (SlotItemUniqueId != 0 && SlotItemUniqueId == selectedItemUniqueId);

        if (Image_Selected != null)
        {
            Image_Selected.gameObject.SetActive(isSelected);
        }

        return isSelected;
    }

    public long GetSelectedItemUniqueId()
    {
        return SlotItemUniqueId;
    }

    public void ClearSlot()
    {
        SlotItemUniqueId = 0;
        IsUsableItem = false;

        if (Image_Icon != null) Image_Icon.sprite = null;
        if (Text_StackCount != null) Text_StackCount.text = string.Empty;
        if (Image_Selected != null) Image_Selected.gameObject.SetActive(false);

        gameObject.SetActive(false);
    }
}
