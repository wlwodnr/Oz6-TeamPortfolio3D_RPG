using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ItemDataBase 
{
    public static Dictionary<string, TreasureDropData> TreasureDataDic = new Dictionary<string, TreasureDropData>();
    public static Dictionary<string,ItemBase> ItemDataDic = new Dictionary<string,ItemBase>();
    public static Dictionary<string,Sprite> ItemIconDic = new Dictionary<string,Sprite>();
    public static Dictionary<string,StoreData> StoreDic = new Dictionary<string,StoreData>();  //<NpcID,StoreData>
    public static Sprite BaseIcon;


    public static void LoadAllData()
    {
        ItemBase[] statItems = Resources.LoadAll<ItemBase>("ItemData/StatUpItem");
        ItemBase[] consumeableItems = Resources.LoadAll<ConsumableItem>("ItemData/ConsumableItem");
        StoreData[] storeDatas = Resources.LoadAll<StoreData>("StoreData");
        TreasureDropData[] treasureDatas = Resources.LoadAll<TreasureDropData>("TreasureData");

        foreach(var item in statItems)
        {
            if(ItemDataDic.ContainsKey(item.ItemId) == false)
            {
                ItemDataDic.Add(item.ItemId, item);
                Sprite icon = Resources.Load<Sprite>(item.Iconpath);
                if(icon != null)
                {
                    ItemIconDic.Add(item.ItemId, icon);
                }
            }
            else
            {
                Debug.LogWarning($"중복 데이터 {item.name}");
            }
        }

        foreach (var data in storeDatas)
        {
            if (StoreDic.ContainsKey(data.NpcId) == false)
            {
                StoreDic.Add(data.NpcId, data);
            }
            else
            {
                Debug.LogWarning($"중복 데이터 {data.name}");
            }
        }


        foreach (var item in consumeableItems)
        {
            if (ItemDataDic.ContainsKey(item.ItemId) == false)
            {
                ItemDataDic.Add(item.ItemId, item);
                Sprite icon = Resources.Load<Sprite>(item.Iconpath);
                if (icon != null)
                {
                    ItemIconDic.Add(item.ItemId, icon);
                }
            }
            else
            {
                Debug.LogWarning($"중복 데이터 {item.name}");
            }
        }

        foreach(var data in treasureDatas)
        {
            if(TreasureDataDic.ContainsKey(data.TreasureId) == false)
            {
                TreasureDataDic.Add(data.TreasureId, data);
            }
            else
            {
                Debug.LogWarning($"중복 데이터 {data.TreasureId}");
            }
        }
        Debug.Log($"[ItemDataBase] itemdata - {ItemDataDic.Count} icondata - " +
            $"{ItemIconDic.Count} storedata - {StoreDic.Count} TreasureData - {TreasureDataDic.Count} 로딩완료");
    }

    public static ItemBase GetItemData(string key)
    {
        if (ItemDataDic.ContainsKey(key))
        {
            return ItemDataDic[key];
        }
        else return null;
    }

    public static Sprite GetItemIcon(string key)
    {
        if (ItemIconDic.ContainsKey(key))
        {
            return ItemIconDic[key];
        }
        else return null;
    }
    
    public static ItemBase GetRandomItem()
    {
        int randomIndex = Random.Range(0, ItemDataDic.Count);
        string key = ItemDataDic.Keys.ElementAt<string>(randomIndex);
        if(ItemDataDic.ContainsKey(key))
        {
            return ItemDataDic[key];
        }
        Debug.Log("[ItemDataBase] GetRandomItem - 널이 리턴됫어용...");
        return null;
    }
}
