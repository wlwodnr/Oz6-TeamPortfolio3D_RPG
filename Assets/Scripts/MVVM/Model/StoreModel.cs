using System;
using System.Collections.Generic;
using UnityEngine;

public struct StoreItemInfo
{
    public int Price;
    public int Stock;
}
public class StoreModel
{
    private Dictionary<string, StoreItemInfo> _shopInventory = new Dictionary<string, StoreItemInfo>();

    public event Action<string> OnItemChanged;

    public void InitializeInventory(Dictionary<string, StoreItemInfo> initialData)
    {
        _shopInventory = initialData;
    }

    public bool HasStock(string id)
    {
        return _shopInventory.TryGetValue(id, out var info) && info.Stock > 0;
    }

    public void ReduceStock(string id)
    {
        if(_shopInventory.TryGetValue(id, out var info))
        {
            if(info.Stock > 0)
            {
                info.Stock = info.Stock - 1;
                _shopInventory[id] = info;
                OnItemChanged?.Invoke(id);
            }
        }

    }

    public bool TryGetInfo(string id, out StoreItemInfo info)
    {
        return _shopInventory.TryGetValue(id, out info);
    }
}
