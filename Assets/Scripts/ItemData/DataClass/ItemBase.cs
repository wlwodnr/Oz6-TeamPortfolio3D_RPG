using UnityEngine;

public class ItemBase : ScriptableObject
{
    [Header("Base 데이터")]
    [SerializeField] public string ItemId;
    [SerializeField] public string Name;
    [SerializeField] public string Iconpath;
    [SerializeField] public string ItemDesc;
    [SerializeField] public int MaxCount;  // 인벤 한칸에서 최대로 쌓을수있는 갯수
}



public interface ITradeable
{
    public int BuyPrice { get; set; }
    public int SellPrice { get; set; }

    public void Buy();
    public void Sell();
}

public interface IEquipable
{
    EffectType EffectType { get; set; }
}

public interface IUseable
{
    public void Use();
}
