using System;
using System.Collections.Generic;

public class SlotContainerViewModel
{
    private readonly List<SlotViewModel> _slots;
    public event Action<SlotViewModel> OnSelectionChanged;

    private SlotViewModel _selectedSlot;
    public SlotViewModel GetSelectedSlot() => _selectedSlot;

    public SlotContainerViewModel(List<SlotViewModel> slots)
    {
        _slots = slots;
        foreach (var slot in _slots)
        {
            slot.OnSelected += OnSlotSelected;
        }
    }

    private void OnSlotSelected(SlotViewModel vm, bool currentState)
    {
        // 이미 선택된 슬롯을 다시 클릭 -> 선택 해제
        //if (_selectedSlot == vm)
        //{
        //    vm.IsSelected = false;
        //    _selectedSlot = null;
        //    OnSelectionChanged?.Invoke(null);
        //    return;
        //}

        if (_selectedSlot != null)
        {
            _selectedSlot.IsSelected = false;
        }

        vm.IsSelected = true;
        _selectedSlot = vm;
        OnSelectionChanged?.Invoke(null);
    }

    public void Dispose()
    {
        foreach (var slot in _slots)
        {
            slot.OnSelected -= OnSlotSelected;
        }
    }
}