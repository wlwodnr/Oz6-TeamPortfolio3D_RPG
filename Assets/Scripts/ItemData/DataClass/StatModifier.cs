using System;
using UnityEngine;

public enum StatType { MoveSpeed, AttackPower, MaxHP, Defense, AttackSpeed }
public enum ModifierType { Flat, Percent }

[System.Serializable]
public class StatModifier 
{
    public StatType Type;
    public ModifierType ModType;
    public float Value;
}
