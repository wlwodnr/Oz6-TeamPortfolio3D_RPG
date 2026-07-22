using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


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
        else
        {
            Debug.LogWarning($"[{name}] 할당된 ItemEffect가 없습니다.");
        }
    }
    public void Buy()
    {

    }
    public void Sell()
    {

    }

}