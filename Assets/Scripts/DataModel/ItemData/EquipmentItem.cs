using UnityEngine;
using System.Collections.Generic;

public class EquipmentItem : ItemBase, ITradeable, IEquipable
{
    public EquipType ItemType { get; set; }
    public List<StatModifier> Modifiers;
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

    public void Equip()
    {
        // 착용 효과 적용
    }

    public void UnEquip()
    {
        // 착용 효과 해제
    }
}