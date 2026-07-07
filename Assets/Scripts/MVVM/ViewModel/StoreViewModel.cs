using System;
using UnityEngine;

public class StoreViewModel 
{
    private StoreModel _storeModel;
    //private PlayerModel _playerModel;
    //private InventoryModel _inventoryModel;

    public event Action<int> OnGoldChanged;
    public event Action<string> OnItemChanged;

    public StoreViewModel(StoreModel storemodel)
    {
        _storeModel = storemodel;
        //_playerModel = playermodel;
        //_inventoryModel = inventorymodel;

        //_playerModel.OnGoldChanged += OnGoldChanged;

        _storeModel.OnItemChanged += OnItemChanged;
    }

    public void RequestPurchase(string id)
    {
        _storeModel.ReduceStock(id);
    }

    public bool TryGetItemData(string id, out StoreItemInfo itemInfo)
    {
        return _storeModel.TryGetInfo(id, out itemInfo);
    }
}
