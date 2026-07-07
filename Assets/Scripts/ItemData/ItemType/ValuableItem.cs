using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ValuableItem", menuName = "ItemData/ValuableItem")]
public class ValuableItem : ItemBase, ITradeable
{

    public int BuyPrice { get; set; }
    public int SellPrice { get; set; }

    public void Buy()
    {
        // 구매 로직
    }
    public void Sell()
    {
        // 판매 로직
    }
}