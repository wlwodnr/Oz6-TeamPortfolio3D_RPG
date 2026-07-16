using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class InventoryUI : UIBase
{
    [SerializeField] private UIButton Button_UseSelectItem;
    [SerializeField] private UIButton Button_CloseSelf;
    [SerializeField] private UIButton Button_CloseSelfAllArea;
    [SerializeField] private List<InventorySlotUI> _fixedSlotList = new List<InventorySlotUI>();

    private InventoryModel _invenVm;
    private SlotContainerViewModel _slotContainerVm;

    private void OnEnable()
    {
        Button_UseSelectItem.BindOnClickButtonEvent(OnClick_UseSelectItem, true);
        Button_CloseSelf.BindOnClickButtonEvent(OnClick_ClosePopup);
        Button_CloseSelfAllArea.BindOnClickButtonEvent(OnClick_ClosePopup);

        SetInventoryItemSlotOnEnable();
        ActiveUseSelectItemButton(false);
    }

    private void OnDisable()
    {
        Button_UseSelectItem.UnBindAllOnClickButtonEvent();
        Button_CloseSelf.UnBindAllOnClickButtonEvent();
        Button_CloseSelfAllArea.UnBindAllOnClickButtonEvent();
    }

    private void OnDestroy()
    {
        if (_invenVm != null)
        {
            _invenVm.PropertyChanged -= OnPropChanged_InvenView;
        }
        DisposeSlotContainer();
    }

    private void SetInventoryItemSlotOnEnable()
    {
        RemoveAllItemSlot();
        FindInventoryViewModelAndBind();
    }

    private void FindInventoryViewModelAndBind()
    {
        var invenVm = NetworkManager.Inst.InventoryService.GetLocalPlayerInvnetoryModel();
        if (invenVm == null || invenVm.ItemList == null || invenVm.ItemList.Count == 0)
        {
            Debug.LogWarning("보유한 아이템이 없습니다!");
            return;
        }

        // 중복 구독 방지를 위해 기존 이벤트 해제 후 재구독
        if (_invenVm != null)
        {
            _invenVm.PropertyChanged -= OnPropChanged_InvenView;
        }

        _invenVm = invenVm;
        _invenVm.PropertyChanged += OnPropChanged_InvenView;
        _invenVm.InvokeOnceOnInit();
    }

    private void OnPropChanged_InvenView(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(InventoryModel.ItemList):
            case "ItemListAdded":
            case "ItemListRemoved":
                ResetItemSlotAndCreateAll();
                break;
            case "ItemListUpdated":

                break;
        }
    }

    private void ResetItemSlotAndCreateAll()
    {
        RemoveAllItemSlot();
        DisposeSlotContainer();

        var slotVmList = new List<SlotViewModel>();
        int slotIndex = 0;
        foreach (var itemKv in _invenVm.ItemList)
        {
            if (slotIndex >= _fixedSlotList.Count) break;

            var slotVm = itemKv.Value;
            var slotView = _fixedSlotList[slotIndex];

            slotView.BindSlotViewModel(slotVm);
            slotVmList.Add(slotVm);

            slotIndex++;
        }

        _slotContainerVm = new SlotContainerViewModel(slotVmList);
        _slotContainerVm.OnSelectionChanged += OnSlotSelected;
    }

    private void OnSlotSelected(SlotViewModel selectedVm)
    {
        if (selectedVm == null)
        {
            ActiveUseSelectItemButton(false);
            return;
        }

        var itemData = GameDataManager.Instance.GetItemData(selectedVm.ItemId);
        bool isUsable = (itemData != null && string.IsNullOrEmpty(itemData.UseItemType) == false);
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
    private void DisposeSlotContainer()
    {
        if (_slotContainerVm != null)
        {
            _slotContainerVm.OnSelectionChanged -= OnSlotSelected;
            _slotContainerVm.Dispose();
            _slotContainerVm = null;
        }
    }
}
