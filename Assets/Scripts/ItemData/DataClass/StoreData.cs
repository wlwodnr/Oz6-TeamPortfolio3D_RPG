using System.Collections.Generic;
using System;
using UnityEngine;

public class StoreData : ScriptableObject
{
    public string StoreName;
    public List<StoreItem> StoreItems;
}

[Serializable]
public struct StoreItem
{
    public string ItemId;
    public int Count;
}