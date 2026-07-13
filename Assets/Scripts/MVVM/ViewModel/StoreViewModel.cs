using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class StoreViewModel 
{
    private StoreModel _storeModel;
    //private PlayerModel _playerModel;
    //private InventoryModel _inventoryModel;

    private Dictionary<long, SlotViewModel> _slotvmDic = new Dictionary<long, SlotViewModel>();
    private SlotViewModel _selectedSlot;

    public event Action<string, long> OnSlotChanged;
    public event Action<int> OnGoldChanged;
    public event Action<string> OnItemChanged;

    public StoreViewModel(StoreModel storemodel)
    {
        _storeModel = storemodel;
        //_playerModel = playermodel;
        //_inventoryModel = inventorymodel;

        //_playerModel.OnGoldChanged += OnGoldChanged;


    }
    
    public void InvokeOnceOnInit()
    {
        foreach (var slotModel in _storeModel.GetAllSlots())
        {
            AddSlot(slotModel);
        }
    }

    public void SelectSlot(SlotViewModel slot, bool isSelected)
    {
        if(isSelected == false)
        {
            if (_selectedSlot != null)
            {
                _selectedSlot.IsSelected = false;
            }
            _selectedSlot = slot;
            _selectedSlot.IsSelected = true;
        }
        else
        {
            if(_selectedSlot == slot)
            {
                _selectedSlot.IsSelected = false;
                _selectedSlot = null;
            }
            else
            {
                _selectedSlot.IsSelected = false;
                slot.IsSelected = false;
                _selectedSlot = null;
            }
        }
    }

    public void AddSlot(SlotModel model)
    {
        if(_slotvmDic.ContainsKey(model.SlotId) == false)
        {
            SlotViewModel slotvm = new SlotViewModel(model);
            slotvm.OnSelected += SelectSlot;
            _slotvmDic.Add(model.SlotId, slotvm);
            OnSlotChanged?.Invoke("AddSlot", model.SlotId);
        }
        else
        {
            Debug.Log("[StoreViewModel] 이미 해당 Slot이 존재함");
        }
    }

    public void RemoveSlot(SlotViewModel slot)
    {
        if (_slotvmDic.ContainsKey(slot.GetSlotId()) == false) return; 

        slot.OnSelected -= SelectSlot;
        _slotvmDic.Remove(slot.GetSlotId());
        OnSlotChanged?.Invoke("RemoveSlot", slot.GetSlotId());
    }

    public void Refresh(StoreData newData)
    {
        _selectedSlot = null;
        _storeModel.Refresh(newData);
        ClearSlotVM();
        foreach (var slotModel in _storeModel.GetAllSlots())
        {
            AddSlot(slotModel);
        }
    }

    public void ClearSlotVM()
    {
        foreach(var slotvm in _slotvmDic)
        {
            RemoveSlot(slotvm.Value);
        }
        _slotvmDic.Clear();
    }

    public SlotViewModel GetSlotVM(long slotId)
    {
        if (_slotvmDic.ContainsKey(slotId)) return _slotvmDic[slotId];
        else return null;
    }


}
