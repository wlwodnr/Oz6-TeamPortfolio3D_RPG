using System;
using System.Collections.Generic;
using UnityEngine;

public class SlotModel
{
    public long SlotId;
    public string ItemId;
    public int Count;

    public SlotModel(long slotId, string itemId, int count)
    {
        SlotId = slotId;
        ItemId = itemId;
        Count = count;
    }
}
