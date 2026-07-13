using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EquipmentItem", menuName = "ItemData/EquipmentItem")]
public class EquipmentItem : ItemBase, ITradeable, IEquipable
{
    public EffectType EffectType { get; set; }
    public List<StatModifier> Modifiers;  // 추후 StatModifier부분이 구현되면 수정예정
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