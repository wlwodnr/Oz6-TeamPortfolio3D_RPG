using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TreasureDropData", menuName = "TreasureDrop")]
public class TreasureDropData : ScriptableObject
{
    public string TreasureId;
    public int DropCoins;
    public int ItemDropCount;
}
