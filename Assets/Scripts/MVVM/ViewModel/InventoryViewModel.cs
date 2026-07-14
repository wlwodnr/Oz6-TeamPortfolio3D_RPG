using System.Collections.Generic;
using UnityEngine;

public class InvnetoryViewModel : ViewModelBase
{
    private Dictionary<long, SlotViewModel> _itemList = new Dictionary<long, SlotViewModel>();
    public Dictionary<long, SlotViewModel> ItemList
    {
        get => _itemList;
        set
        {
            if (_itemList != value)
            {
                _itemList = value;
                OnPropertyChanged(nameof(ItemList));
            }
        }
    }

    public void InvokeOnceOnInit()
    {
        OnPropertyChanged(nameof(ItemList));
    }

    public void AddItemSlotViewModel(SlotViewModel slotVm)
    {
        _itemList.Add(slotVm.GetSlotId(), slotVm);
        OnPropertyChanged("ItemListAdded");
    }

    public void RemoveItemSlotViewModel(long uniqueId)
    {
        if (_itemList.ContainsKey(uniqueId) == true)
        {
            _itemList.Remove(uniqueId);
        }

        OnPropertyChanged("ItemListRemoved");
    }

}