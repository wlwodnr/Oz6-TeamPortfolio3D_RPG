using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class InventoryViewModel : ViewModelBase
{
    private InventoryModel _inventoryModel;
    private Dictionary<long, InventorySlotViewModel> _slotViewModels = new Dictionary<long, InventorySlotViewModel>();
    public IReadOnlyDictionary<long, InventorySlotViewModel> SlotViewModels => _slotViewModels;

    private InventorySlotViewModel _selectedSlot;
    public InventorySlotViewModel SelectedSlot => _selectedSlot;
    public event Action<InventorySlotViewModel> OnSelectionChanged;

    public void Initialize(InventoryModel inventoryModel)
    {
        _inventoryModel = inventoryModel;

        ClearAndDisposeSlots();

        foreach(var slotModel in _inventoryModel.GetAllSlots())
        {
            var slotVm = new InventorySlotViewModel(slotModel);

            slotVm.OnSelected += OnSlotSelected;
            slotVm.InvokeOnceOnInit();

            _slotViewModels.Add(slotModel.SlotId, slotVm);
        }

        OnPropertyChanged(nameof(SlotViewModels));
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

    public InventorySlotViewModel GetSlotViewModel(long slotId)
    {
        return _slotViewModels.TryGetValue(slotId, out var slotVm) ? slotVm : null;
    }

    private void ClearAndDisposeSlots()
    {
        foreach (var slotVm in _slotViewModels.Values)
        {
            if (slotVm != null)
            {
                slotVm.OnSelected -= OnSlotSelected;
                slotVm.Dispose();
            }
        }
        _slotViewModels.Clear();
        _selectedSlot = null;
    }

    public void Dispose()
    {
        ClearAndDisposeSlots();
    }
}
