using System;
using System.Collections.Generic;

public class DropItem
{
    private static readonly Random _random = new Random();

    public string ItemDataId { get; private set; }
    public int Count { get; private set; }

    public DropItem(string itemDataId, int count)
    {
        ItemDataId = itemDataId;
        Count = count;
    }

    public static bool TryCreate(Dictionary<string, DropData> dropDataList, string monsterDataId, out DropItem dropItem)
    {
        dropItem = null;

        if (dropDataList == null || string.IsNullOrEmpty(monsterDataId)) return false;

        foreach (DropData dropData in dropDataList.Values)
        {
            if (dropData.MonsterDataId != monsterDataId) continue;
            if (dropData.DropChance <= 0f || dropData.DropChance > 1f) return false;
            if (dropData.MinCount <= 0 || dropData.MaxCount < dropData.MinCount) return false;
            if (dropData.DropChance < 1f && _random.NextDouble() >= dropData.DropChance) return false;

            int count = _random.Next(dropData.MinCount, dropData.MaxCount + 1);
            dropItem = new DropItem(dropData.ItemDataId, count);
            return true;
        }

        return false;
    }
}
