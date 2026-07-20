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
        InventoryData defaultData = new InventoryData(30);
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

        var itemData = ItemDataBase.GetItemData(slot.ItemId);
        if (itemData is IUseable useableItem)
        {
            useableItem.Use();

            RequestRemoveItem(requestUseTargetSlotId, 1);
            return true;
        }
        else if (itemData is IEquipable equipableItem)
        {
            // 장비 착용 로직 예시
            // EquipmentService.Equip(equipableItem);
            return true;
        }

        // 아이템 효과 적용
        // ItemUseHandler.UseItemFunction(itemData.UseItemType, itemData.UseItemParameterList);

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