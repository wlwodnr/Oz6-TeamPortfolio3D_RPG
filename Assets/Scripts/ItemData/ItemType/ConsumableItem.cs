using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "NewItemData", menuName = "Item/ConsumableItem")]
public class ConsumableItem : ItemBase, ITradeable, IUseable
{
    [Header("Trade 데이터")]
    [SerializeField] private int buyPrice;
    [SerializeField] private int sellPrice;

    [Header("아이템 효과")]
    [SerializeField] private ItemEffect effect;

    public int BuyPrice { get => buyPrice; set => buyPrice = value; }
    public int SellPrice { get => sellPrice; set => sellPrice = value; }

    public void Use()
    {
        if (effect != null)
        {
            effect.Apply();
        }
    }
    public void Buy()
    {

    }
    public void Sell()
    {

    }

}