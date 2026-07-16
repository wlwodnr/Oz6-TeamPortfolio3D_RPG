using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class InventoryViewModel : ViewModelBase
{
    private InventoryModel _inventoryModel;

    private Dictionary<long, SlotViewModel> _slotViewModels = new Dictionary<long, SlotViewModel>();
    public IReadOnlyDictionary<long, SlotViewModel> SlotViewModels => _slotViewModels;

    public void Initialize(InventoryModel inventoryModel)
    {
        _inventoryModel = inventoryModel;

        foreach (var slotVm in _slotViewModels.Values)
        {
            slotVm.OnSelected -= OnSlotSelected;
        }

        _slotViewModels.Clear();

        foreach (var slotModel in _inventoryModel.GetAllSlots())
        {
            var slotVm = new SlotViewModel(slotModel);

            slotVm.InvokeOnceOnInit();

            slotVm.OnSelected += OnSlotSelected;

            _slotViewModels.Add(slotModel.SlotId, slotVm);
        }

        OnPropertyChanged(nameof(SlotViewModels));
    }

    private void OnSlotSelected(SlotViewModel selectedSlot, bool isSelected)
    {
        if (isSelected)
        {
            foreach (var slotVm in _slotViewModels.Values)
            {
                if (slotVm != selectedSlot)
                {
                    slotVm.IsSelected = false;
                }
            }
        }
    }

    public SlotViewModel GetSlotViewModel(long slotId)
    {
        return _slotViewModels.TryGetValue(slotId, out var slotVm) ? slotVm : null;
    }
}
