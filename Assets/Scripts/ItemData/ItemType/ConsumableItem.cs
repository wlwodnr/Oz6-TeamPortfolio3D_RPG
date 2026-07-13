using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "NewItemData", menuName = "Item/ConsumableItem")]
public class ConsumableItem : ItemBase, ITradeable, IUseable
{
    public int BuyPrice { get; set; }
    public int SellPrice { get; set; }

    public void Use()
    {

    }
    public void Buy()
    {

    }
    public void Sell()
    {

    }

}