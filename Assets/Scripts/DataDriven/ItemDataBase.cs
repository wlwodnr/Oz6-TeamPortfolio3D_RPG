using System.Collections.Generic;
using UnityEngine;

public static class ItemDataBase 
{
    public static Dictionary<string,ItemBase> ItemDataDic = new Dictionary<string,ItemBase>();
    public static Dictionary<string,Sprite> ItemIconDic = new Dictionary<string,Sprite>();

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
