using System;
using System.Collections.Generic;
using UnityEngine;

public class SlotModel
{
    private long _slotId;
    private string _itemId;
    private int _count;

    public event Action<string> OnSlotChanged;

    public SlotModel(long slotId, string itemId, int count)
    {
        _slotId = slotId;
        _itemId = itemId;
        _count = count;
    }
    public long SlotId => _slotId;
    public string ItemId
    {
        get => _itemId;
        set { _itemId = value; OnSlotChanged?.Invoke(nameof(ItemId)); }
    }

    public int Count
    {
        get => _count;
        set { _count = value; OnSlotChanged?.Invoke(nameof(Count)); }
    }
}
