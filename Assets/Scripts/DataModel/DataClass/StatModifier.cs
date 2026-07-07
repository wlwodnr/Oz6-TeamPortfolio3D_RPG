using UnityEngine;

public enum StatType { MoveSpeed, AttackPower, MaxHP, Defense }
public enum ModifierType { Flat, Percent }
public class StatModifier 
{
    public StatType Type;
    public ModifierType ModType;
    public float Value;
}
