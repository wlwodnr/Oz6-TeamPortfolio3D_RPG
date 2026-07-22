using System.Collections.Generic;
using UnityEngine;

public class NetworkInventoryService
{
    private InventoryModel _localPlayerInventoryModel;

    public InventoryModel GetLocalPlayerInventoryModel()
    {
        if (_localPlayerInventoryModel == null)
        {
            CreateLocalPlayerInventoryModel();
        }

        return _localPlayerInventoryModel;
    }

    private InventoryModel CreateLocalPlayerInventoryModel()
    {
        InventoryData defaultData = new InventoryData();
        _localPlayerInventoryModel = new InventoryModel(defaultData);
        return _localPlayerInventoryModel;
    }

    public bool ReuestAddItem(string itemDataId, int addItemCount)
    {
        var model = GetLocalPlayerInventoryModel();
        
        bool isSuccess = model.InputItem(itemDataId, addItemCount);

        if (isSuccess)
        {
            // NetworkManager.Inst.SaveLoadService.RequstSaveData();
        }

        return isSuccess;
    }


    public bool RequestUseItem(long requestUseTargetSlotId)
    {
        var model = GetLocalPlayerInventoryModel();
        var slot = model.GetSlot(requestUseTargetSlotId);

        if (slot == null || string.IsNullOrEmpty(slot.ItemId))
        {
            return false;
        }

        bool isSuccess = ItemUseHandler.Execute(slot.ItemId);

        if (isSuccess)
        {
            RequestRemoveItem(requestUseTargetSlotId, 1);
            return true;
        }

        return false;
    }

    private void RequestRemoveItem(long slotId, int count)
    {
        var model = GetLocalPlayerInventoryModel();
        var slot = model.GetSlot(slotId);

        if (slot == null) return;

        int remainCount = slot.Count - count;

        if (remainCount <= 0)
        {
            model.ClearSlot(slotId);
        }
        else
        {
            model.TrySetItem(slotId, slot.ItemId, remainCount);
        }
        
        // NetworkManager.Inst.SaveLoadService.RequstSaveData();
    }

    public IEnumerable<InventorySlotModel> GetPlayerItemList()
    {
        var model = GetLocalPlayerInventoryModel();
        return model.GetAllSlots();
    }

}