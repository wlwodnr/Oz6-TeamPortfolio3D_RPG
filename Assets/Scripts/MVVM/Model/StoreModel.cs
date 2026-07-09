using System;
using System.Collections.Generic;
using UnityEngine;


public class StoreModel
{
    private string _storeName;
    private List<SlotModel> _slots = new List<SlotModel>();


    public StoreModel(StoreData storeData)
    {
        if(storeData == null)
        {
            return;
        }

        _storeName = storeData.StoreName;

        foreach(var item in storeData.StoreItems)
        {
            _slots.Add(new SlotModel(GameUtil.GenerateUniqueId(), item.ItemId, item.Count));
        }
    }

    public void Refresh(StoreData storeData)
    {
        if (storeData == null)
        {
            return;
        }

        _slots.Clear();

        _storeName = storeData.StoreName;
        foreach(var item in storeData.StoreItems)
        {
            _slots.Add(new SlotModel(GameUtil.GenerateUniqueId(), item.ItemId, item.Count));
        }
    }

    public IEnumerable<SlotModel> GetAllSlots() => _slots;
}
