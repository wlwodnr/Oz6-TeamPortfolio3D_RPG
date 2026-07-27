using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemUseHandler
{
    public static bool Execute(string itemId, GameObject target = null)
    {
        var itemData = ItemDataBase.GetItemData(itemId);

        if (itemData == null)
        {
            Debug.LogWarning($"[ItemUseHandler] 아이템 데이터를 찾을 수 없습니다: {itemId}");
            return false;
        }

        if (itemData is IUseable useableItem)
        {
            useableItem.Use();
            return true;
        }

        else if (itemData is IEquipable)
        {
            if (NetworkManager.Inst == null || NetworkManager.Inst.LocalPlayerService == null)
            {
                Debug.LogWarning($"[ItemUseHandler] LocalPlayerService가 없어 장비 아이템을 사용할 수 없습니다: {itemId}");
                return false;
            }

            PlayerModel localPlayerModel = NetworkManager.Inst.LocalPlayerService.GetLocalPlayerModel();

            if (localPlayerModel == null)
            {
                Debug.LogWarning($"[ItemUseHandler] PlayerModel을 찾을 수 없어 장비 아이템을 사용할 수 없습니다: {itemId}");
                return false;
            }

            localPlayerModel.Additem(itemId);
            return true;
        }

        return false;
    }
}
