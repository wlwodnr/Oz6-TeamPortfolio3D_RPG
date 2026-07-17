using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class InventoryUI : UIBase
{
    [SerializeField] private UIButton Button_UseSelectItem;
    [SerializeField] private UIButton Button_CloseSelf;
    [SerializeField] private UIButton Button_CloseSelfAllArea;
    [SerializeField] private List<InventorySlotUI> _fixedSlotList = new List<InventorySlotUI>();

    private InventoryViewModel _invenVm;
    private InventorySlotContainerViewModel _slotContainerVm;

    private void OnEnable()
    {
        Button_UseSelectItem.BindOnClickButtonEvent(OnClick_UseSelectItem, true);
        Button_CloseSelf.BindOnClickButtonEvent(OnClick_ClosePopup);
        Button_CloseSelfAllArea.BindOnClickButtonEvent(OnClick_ClosePopup);

        SetInventoryItemSlotOnEnable();
        ActiveUseSelectItemButton(false);

        //SetCursorUnlock(true);
    }

    private void OnDisable()
    {
        Button_UseSelectItem.UnBindAllOnClickButtonEvent();
        Button_CloseSelf.UnBindAllOnClickButtonEvent();
        Button_CloseSelfAllArea.UnBindAllOnClickButtonEvent();

        UnbindInventoryViewModel();

        //SetCursorUnlock(false);
    }

    private void OnDestroy()
    {
        UnbindInventoryViewModel();
    }

    private void SetInventoryItemSlotOnEnable()
    {
        RemoveAllItemSlot();
        FindInventoryViewModelAndBind();
    }

    private void FindInventoryViewModelAndBind()
    {
        var invenModel = NetworkManager.Inst.InventoryService.GetLocalPlayerInventoryModel();
        if (invenModel == null)
        {
            Debug.LogWarning("보유한 아이템이 없습니다!");
            return;
        }

        UnbindInventoryViewModel();

        _invenVm = new InventoryViewModel();
        _invenVm.Initialize(invenModel);
        _invenVm.PropertyChanged += OnPropChanged_InvenView;

        ResetItemSlotAndCreateAll();
    }

    private void OnPropChanged_InvenView(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(InventoryViewModel.SlotViewModels))
        {
            ResetItemSlotAndCreateAll();
        }
    }

    private void ResetItemSlotAndCreateAll()
    {
        RemoveAllItemSlot();
        ActiveUseSelectItemButton(false);

        if (_invenVm == null || _invenVm.SlotViewModels == null) return;

        var slotVmList = new List<InventorySlotViewModel>();
        
        int slotIndex = 0;
        foreach (var itemKv in _invenVm.SlotViewModels)
        {
            if (slotIndex >= _fixedSlotList.Count) break;

            var slotVm = itemKv.Value;
            var slotView = _fixedSlotList[slotIndex];

            slotView.BindSlotViewModel(slotVm);
            slotVmList.Add(slotVm);

            slotIndex++;
        }
    }

    private void OnSlotSelected(InventorySlotViewModel selectedVm)
    {
        if (selectedVm == null || string.IsNullOrEmpty(selectedVm.ItemId))
        {
            ActiveUseSelectItemButton(false);
            return;
        }

        var itemData = ItemDataBase.GetItemData(selectedVm.ItemId);
        // IUseable 인터페이스 구현 여부로 사용 가능 아이템 판단
        bool isUsable = (itemData is IUseable);
        ActiveUseSelectItemButton(isUsable);
    }

    private void ActiveUseSelectItemButton(bool isActive)
    {
        if (Button_UseSelectItem != null)
        {
            Button_UseSelectItem.gameObject.SetActive(isActive);
        }
    }

    private void RequestSelectedUseItem()
    {
        var selected = _slotContainerVm?.GetSelectedSlot();
        if (selected == null) return;

        NetworkManager.Inst.InventoryService.RequestUseItem(selected.GetSlotId());
    }

    public void OnClick_ClosePopup()
    {
        UIManager.Instance.CloseContentUI(UIType.InventoryUI);
    }

    public void OnClick_UseSelectItem()
    {
        RequestSelectedUseItem();
    }

    private void RemoveAllItemSlot()
    {
        foreach (var slotView in _fixedSlotList)
        {
            if (slotView != null)
            {
                slotView.ClearSlot();
            }
        }
    }
    private void UnbindInventoryViewModel()
    {
        if (_invenVm != null)
        {
            _invenVm.PropertyChanged -= OnPropChanged_InvenView;
            _invenVm.OnSelectionChanged -= OnSlotSelected;
            _invenVm.Dispose();
            _invenVm = null;
        }
    }
}
