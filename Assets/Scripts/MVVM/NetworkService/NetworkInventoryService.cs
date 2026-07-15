using System.Collections.Generic;
using UnityEngine;

public class NetworkInventoryService
{
    private InvnetoryViewModel _localPlayerInventoryViewModel;

    public InvnetoryViewModel GetLocalPlayerInvnetoryViewModel()
    {
        if (_localPlayerInventoryViewModel == null)
        {
            CreateLocalPlayerInvnetoryViewModel();
        }

        return _localPlayerInventoryViewModel;
    }

    private InvnetoryViewModel CreateLocalPlayerInvnetoryViewModel()
    {
        var inventoryVm = new InvnetoryViewModel();
        _localPlayerInventoryViewModel = inventoryVm;
        return inventoryVm;
    }

    public void AddItem(string itemDataId, int addItemCount)
    {
        long uniqueId = GameUtil.GenerateUniqueId();

        var newItemVm = new SlotViewModel(new SlotModel(uniqueId, itemDataId, addItemCount)); 
        newItemVm.ItemId = itemDataId;
        newItemVm.Count = addItemCount;

        var invenVm = GetLocalPlayerInvnetoryViewModel();
        invenVm.AddItemSlotViewModel(newItemVm);

       //NetworkManager.Inst.SaveLoadService.RequstSaveData();
    }


    public bool RequestUseItem(long requestUseTargetSlotId)
    {
        var invenVm = GetLocalPlayerInvnetoryViewModel();

        foreach (var itemSlotKv in invenVm.ItemList)
        {
            var itemSlotVm = itemSlotKv.Value;
            if (itemSlotVm.GetSlotId() == requestUseTargetSlotId)
            {
                var itemData = GameDataManager.Instance.GetItemData(itemSlotVm.ItemId);
                if (itemData != null && string.IsNullOrEmpty(itemData.UseItemType) == false)
                {
                    //ItemUseHandler.UseItemFunction(itemData.UseItemType, itemData.UseItemParameterList);
                }

                RequestRemoveItem(itemSlotVm.GetSlotId());
                break;
            }
        }

        return true;
    }

    private void RequestRemoveItem(long removeTargetUniqueId)
    {
        var invenVm = GetLocalPlayerInvnetoryViewModel();
        invenVm.RemoveItemSlotViewModel(removeTargetUniqueId);

        //NetworkManager.Inst.SaveLoadService.RequstSaveData();
    }

    public Dictionary<long, SlotViewModel> GetPlayerItemList()
    {
        var invenVm = GetLocalPlayerInvnetoryViewModel();
        return invenVm.ItemList;
    }


}