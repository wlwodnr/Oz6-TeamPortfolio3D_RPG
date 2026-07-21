using System;
using System.Collections.Generic;

public class InventoryModel
{
    private const string EMPTY_ITEM_ID = "";

    private int _maxSlotCount;
    private Dictionary<long, InventorySlotModel> _slots = new Dictionary<long, InventorySlotModel>();

    public InventoryModel(InventoryData inventoryData)
    {
        if (inventoryData == null)
        {
            return;
        }

        Init(inventoryData);
    }

    public void Refresh(InventoryData inventoryData)
    {
        if (inventoryData == null)
        {
            return;
        }

        _slots.Clear();
        Init(inventoryData);
    }

    private void Init(InventoryData inventoryData)
    {
        _maxSlotCount = inventoryData.MaxSlotCount;

        for (long i = 0; i < _maxSlotCount; i++)
        {
            _slots.Add(i, new InventorySlotModel(i, EMPTY_ITEM_ID, 0));
        }

        foreach (var item in inventoryData.InventoryItems)
        {
            if (_slots.TryGetValue(item.SlotIndex, out var slot))
            {
                slot.ItemId = item.ItemId;
                slot.Count = item.Count;
            }
        }
    }

    public bool TrySetItem(long slotId, string itemId, int count)
    {
        if (!_slots.TryGetValue(slotId, out var slot))
        {
            return false;
        }

        slot.ItemId = itemId;
        slot.Count = count;
        return true;
    }

    public bool InputItem(string itemId, int count)
    {
        if (string.IsNullOrEmpty(itemId) || count <= 0) return false;

        var itemData = ItemDataBase.GetItemData(itemId);
        if (itemData == null) return false;

        int remainingCount = count;
        int maxCount = itemData.MaxCount;

        foreach (var slot in _slots.Values)
        {
            if (slot.ItemId == itemId && slot.Count < maxCount)
            {
                int spaceLeft = maxCount - slot.Count;
                int addCount = Math.Min(remainingCount, spaceLeft);

                slot.Count += addCount;
                remainingCount -= addCount;

                if (remainingCount <= 0)
                {
                    return true;
                }
            }
        }

        while (remainingCount > 0)
        {
            if (TryGetFirstEmptySlotId(out long emptySlotId))
            {
                int addCount = Math.Min(remainingCount, maxCount);
                TrySetItem(emptySlotId, itemId, addCount);
                remainingCount -= addCount;
            }
            else
            {
                // (필요 시 남은 수량을 필드에 기록하거나 Drop 처리)
                return false;
            }
        }

        return true;
    }

    public bool ClearSlot(long slotId)
    {
        return TrySetItem(slotId, EMPTY_ITEM_ID, 0);
    }

    public bool TryGetFirstEmptySlotId(out long slotId)
    {
        foreach (var kvp in _slots)
        {
            if (kvp.Value.ItemId == EMPTY_ITEM_ID)
            {
                slotId = kvp.Key;
                return true;
            }
        }

        slotId = -1;
        return false;
    }

    public InventorySlotModel GetSlot(long slotId)
    {
        return _slots.TryGetValue(slotId, out var slot) ? slot : null;
    }

    public IEnumerable<InventorySlotModel> GetAllSlots() => _slots.Values;

    public InventoryData CaptureInventoryData()
    {
        InventoryData saveData = new InventoryData(_maxSlotCount);

        foreach(var item in _slots)
        {
            saveData.InventoryItems.Add(new InventoryItemData(item.Key, item.Value.ItemId, item.Value.Count));
        }

        return saveData;
    }
}
