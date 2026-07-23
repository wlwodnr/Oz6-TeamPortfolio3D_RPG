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

        else if (itemData is IEquipable equipableItem)
        {
            // EquipmentService.Equip(equipableItem);
            return true;
        }

        return false;
    }
}
