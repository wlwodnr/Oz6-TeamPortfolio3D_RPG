using System;
using System.Collections.Generic;

public class InventorySlotContainerViewModel
{
    private readonly List<InventorySlotViewModel> _slots;
    public event Action<InventorySlotViewModel> OnSelectionChanged;

    private InventorySlotViewModel _selectedSlot;
    public InventorySlotViewModel GetSelectedSlot() => _selectedSlot;

    public InventorySlotContainerViewModel(List<InventorySlotViewModel> slots)
    {
        _slots = slots;
        foreach (var slot in _slots)
        {
            slot.OnSelected += OnSlotSelected;
        }
    }

    private void OnSlotSelected(InventorySlotViewModel clickedVm, bool isSelected)
    {
        // 이미 선택된 슬롯을 다시 클릭 -> 선택 해제
        //if (_selectedSlot == clickedVm)
        //{
        //    clickedVm.IsSelected = false;
        //    _selectedSlot = null;
        //    OnSelectionChanged?.Invoke(null);
        //    return;
        //}

        if (_selectedSlot != null)
        {
            _selectedSlot.IsSelected = false;
        }

        clickedVm.IsSelected = true;
        _selectedSlot = clickedVm;
        OnSelectionChanged?.Invoke(clickedVm);
    }

    public void Dispose()
    {
        foreach (var slot in _slots)
        {
            slot.OnSelected -= OnSlotSelected;
        }
        _selectedSlot = null;
    }
}