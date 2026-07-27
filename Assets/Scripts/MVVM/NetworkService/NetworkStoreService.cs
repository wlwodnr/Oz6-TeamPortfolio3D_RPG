using UnityEngine;

public class NetworkStoreService 
{
    private PlayerModel _playerModel;
    private StoreModel _storeModel;

    public void Initialize(PlayerModel playerModel, StoreModel storeModel)
    {
        _playerModel = playerModel;
        _storeModel = storeModel;
    }



    public void RequestItemBuy(long slotId)
    {
        SlotModel slot = _storeModel.GetSlots(slotId);

        if(slot == null)
        {
            Debug.LogError("[NetworkStoreService] Slot null");
        }

        if(slot.Count <= 0)  // 재고 부족
        {
            return;
        }

        var item = ItemDataBase.GetItemData(slot.ItemId);

        if(item is ITradeable items)
        {
            int price = items.BuyPrice;
            if(price > _playerModel.Info.Coins)
            {
                return;
            }
            else
            {
                slot.Count -= 1;
                _playerModel.Info.Coins -= price;
                // 일단 인벤토리에 넣긴했지만 스탯아이템도 인벤토리에 넣을건지?? 아니면 따로 보여주는 창을 만들건지 확인해야함
                NetworkManager.Inst.InventoryService.RequestAddItem(item.ItemId, 1);
                _playerModel.Additem(item.ItemId);
            }
        }
        else
        {
            Debug.LogError("[NetworkStoreService] 잘못된 아이템 데이터");
        }


    }

    public void RequestItemSell(string itemId)
    {
        // 일단 비워뒀음
    }
}
