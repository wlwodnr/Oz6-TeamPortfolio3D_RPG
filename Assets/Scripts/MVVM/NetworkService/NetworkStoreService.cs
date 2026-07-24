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
                // 여기에 인벤토리에 추가 로직 넣기 or 인벤토리 꽉찼는지 검사
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
