using System;
using System.Collections.Generic;
using UnityEngine;


public class StoreModel
{
    private string _storeName;
    private Dictionary<long, SlotModel> _slots = new Dictionary<long, SlotModel>();

    public event Action<string, string> OnPlayerStatsChanged;

    public StoreModel(StoreData storeData)
    {
        if(storeData == null)
        {
            return;
        }

        _storeName = storeData.StoreName;

        foreach(var item in storeData.StoreItems)
        {
            long id = GameUtil.GenerateUniqueId();
            _slots.Add(id, new SlotModel(id, item.ItemId, item.Count));
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
            long id = GameUtil.GenerateUniqueId();
            _slots.Add(id, new SlotModel(id, item.ItemId, item.Count));
        }
    }
    public void RemoveSlot(long id)
    {
        _slots.Remove(id);
    }
   
    public void AddSlot()
    {
        // 나중에 스토어데이터 만드는 로직을 구현하면 작성 (아예 AddSlot은 안쓸수도있음)
    }

    public IEnumerable<SlotModel> GetAllSlots() => _slots.Values;
}
