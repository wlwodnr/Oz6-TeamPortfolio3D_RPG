using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStoreData", menuName = "StoreData")]
public class StoreData : ScriptableObject
{
    public string NpcId;
    public string StoreName;
    public List<StoreItem> StoreItems;
}

[Serializable]
public struct StoreItem
{
    public string ItemId;
    public int Count;
}