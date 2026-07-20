using Cysharp.Threading.Tasks;
using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Text Text_StackCount;
    [SerializeField] private UIButton Button_Slot;
    [SerializeField] private Image Image_Icon;
    [SerializeField] private Image Image_Frame;
    [SerializeField] private Image Image_Selected;

    private InventorySlotViewModel _vm;

    public long SlotItemUniqueId { get; private set; }
    public bool IsUsableItem { get; private set; }

    private void OnEnable()
    {
        Image_Selected.gameObject.SetActive(false);
        Button_Slot.BindOnClickButtonEvent(OnClick_SelectItem);
    }

    private void OnDisable()
    {
        if (Button_Slot != null)
        {
            Button_Slot.UnBindAllOnClickButtonEvent();
        }
        UnbindViewModel();
    }

    public void BindSlotViewModel(InventorySlotViewModel slotVm)
    {
        UnbindViewModel();

        if (slotVm == null)
        {
            ClearSlot();
            return;
        }

        gameObject.SetActive(true);
        _vm = slotVm;
        SlotItemUniqueId = _vm.GetSlotId();
        _vm.PropertyChanged += OnPropChanged_View;
        _vm.InvokeOnceOnInit();
    }

    private void UnbindViewModel()
    {
        if (_vm != null)
        {
            _vm.PropertyChanged -= OnPropChanged_View;
            _vm = null;
        }
    }

    private void OnPropChanged_View(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(InventorySlotViewModel.ItemId):
                UpdateItemIcon();
                break;
            case nameof(InventorySlotViewModel.Count):
                if (Text_StackCount != null)
                {
                    Text_StackCount.text = _vm.Count > 1 ? $"{_vm.Count}" : string.Empty;
                }
                break;
            case nameof(InventorySlotViewModel.IsSelected):
                if (Image_Selected != null)
                {
                    Image_Selected.gameObject.SetActive(_vm.IsSelected);
                }
                break;
        }
    }

    public void UpdateItemIcon()
    {
        if (_vm == null || Image_Icon == null) return;

        Sprite iconSprite = _vm.GetItemIconImage();

        if (iconSprite != null && !string.IsNullOrEmpty(_vm.ItemId))
        {
            Image_Icon.sprite = iconSprite;
            Image_Icon.enabled = true;
        }
        else
        {
            Image_Icon.sprite = null;
            Image_Icon.enabled = false;
        }
    }

    public void OnClick_SelectItem()
    {
        _vm?.ButtonClicked();
        Debug.Log($"{SlotItemUniqueId} 슬롯 클릭됨");
    }

    public void ClearSlot()
    {
        if (_vm != null)
        {
            _vm.IsSelected = false;
        }
        UnbindViewModel();

        SlotItemUniqueId = 0;
        IsUsableItem = false;

        if (Image_Icon != null)
        {
            Image_Icon.sprite = null;
        }
        if (Text_StackCount != null)
        {
            Text_StackCount.text = string.Empty;
        } 
        gameObject.SetActive(false);
    }
}
