using UnityEngine;
using System.Collections.Generic;

public class ConsumableItem : ItemBase, ITradeable, IUseable
{
    public int BuyPrice { get; set; }
    public int SellPrice { get; set; }
    public List<ItemEffect> EffectList = new List<ItemEffect>();

    public void Buy()
    {
        // 구매 로직
    }
    public void Sell()
    {
        // 판매 로직
    }
    public void Use()
    {
        foreach(var effect in EffectList)
        {
            effect.Apply();
        }
    }
}