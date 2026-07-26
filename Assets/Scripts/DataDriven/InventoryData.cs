using System;
using System.Collections.Generic;


[Serializable]
public class InventoryItemData
{
    public long SlotIndex;
    public string ItemId;
    public int Count;

    public InventoryItemData(long slotIndex, string itemId, int count)
    {
        SlotIndex = slotIndex;
        ItemId = itemId;
        Count = count;
    }
}


[Serializable]
public class InventoryData
{
    public int MaxSlotCount;

    public List<InventoryItemData> InventoryItems;

    public InventoryData()
    {
        MaxSlotCount = 30;
        InventoryItems = new List<InventoryItemData>();
    }
}
