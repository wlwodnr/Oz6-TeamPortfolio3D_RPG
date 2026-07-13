using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "NewItemData", menuName = "Item/OnHitItem")]
public class OnHitEffectItem : ItemBase, ITradeable, IEquipable
{
    [Header("Trade 데이터")]
    [SerializeField] private int buyPrice;
    [SerializeField] private int sellPrice;

    [Header("장비 데이터")]
    [SerializeField] public int value;
    [SerializeField] private EffectType effectType;

    public int BuyPrice { get => buyPrice; set => buyPrice = value; }
    public int SellPrice { get => sellPrice; set => sellPrice = value; }
    public EffectType EffectType { get => effectType; set => effectType = value; }


    public void Buy()
    {

    }
    public void Sell()
    {

    }
}
