using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

// 관리주체 역할
public class InventoryUI : UIBase
{
    [SerializeField] private UIButton Button_UseSelectItem;
    [SerializeField] private UIButton Button_CloseSelf;
    [SerializeField] private UIButton Button_CloseSelfAllArea;
    [SerializeField] private List<InventorySlotUI> _fixedSlotList = new List<InventorySlotUI>();

    private Dictionary<long, InventorySlotUI> _itemSlotList = new Dictionary<long, InventorySlotUI>();
    private long _currentSelectedItemUniqueId;

    private InvnetoryViewModel _invenVm;

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
    }

    private void SetInventoryItemSlotOnEnable()
    {
        RemoveAllItemSlot();
        FindInventoryViewModelAndBind();
    }

    private void FindInventoryViewModelAndBind()
    {
        var invenVm = NetworkManager.Inst.InventoryService.GetLocalPlayerInvnetoryViewModel();
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
            case nameof(InvnetoryViewModel.ItemList):
            case "ItemListAdded":
            case "ItemListRemoved":
                ResetItemSlotAndCreateAll();
                break;
            case "ItemListUpdated":
                // 수량 및 정보 갱신 처리 영역
                break;
        }
    }

    private void ResetItemSlotAndCreateAll()
    {
        RemoveAllItemSlot();

        int slotIndex = 0;
        foreach (var itemKv in _invenVm.ItemList)
        {
            if (slotIndex >= _fixedSlotList.Count) break;

            var slotVm = itemKv.Value;
            var slotView = _fixedSlotList[slotIndex];

            slotView.BindSlotViewModel(slotVm);
            slotView.BindSlotSelectEvent(OnChildSlotSelected);

            slotIndex++;
        }
    }

    private void OnChildSlotSelected(long selectedItemUniqueId)
    {
        foreach (var slot in _fixedSlotList)
        {
            if (slot == null) continue;
            bool isSelected = slot.ChangeSelectedState(selectedItemUniqueId);

            if (isSelected == true)
            {
                _currentSelectedItemUniqueId = slot.GetSelectedItemUniqueId();
                ActiveUseSelectItemButton(slot.IsUsableItem);
            }
        }
        Debug.LogWarning($"자식 슬롯 {selectedItemUniqueId} 선택됨!");
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
        NetworkManager.Inst.InventoryService.RequestUseItem(_currentSelectedItemUniqueId);
    }

    public void OnClick_ClosePopup()
    {
        UIManager.Instance.CloseContentUI(UIType.Inventory);
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
}
