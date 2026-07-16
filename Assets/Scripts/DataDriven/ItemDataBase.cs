using System.Collections.Generic;
using UnityEngine;

public static class ItemDataBase 
{
    public static Dictionary<string,ItemBase> ItemDataDic = new Dictionary<string,ItemBase>();
    public static Dictionary<string,Sprite> ItemIconDic = new Dictionary<string,Sprite>();
    public static Dictionary<string,StoreData> StoreDic = new Dictionary<string,StoreData>();  //<NpcID,StoreData>
    public static Sprite BaseIcon;


    public static void LoadAllData()
    {
        ItemBase[] statItems = Resources.LoadAll<ItemBase>("ItemData/StatUpItem");
        ItemBase[] consumeableItems = Resources.LoadAll<ConsumableItem>("ItemData/ConsumableItem");
        StoreData[] storeDatas = Resources.LoadAll<StoreData>("StoreData");

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
        Debug.Log($"[ItemDataBase] itemdata - {ItemDataDic.Count} icondata - {ItemIconDic.Count} storedata - {StoreDic.Count} 로딩완료");
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
}
