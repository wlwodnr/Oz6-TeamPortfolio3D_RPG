using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PassiveSkillData : GameDataBase
{
    public string Name;
    public string Description;
    public string IconPath;
    public string TargetMode;
    public int RequiredLevel;
    public string[] RequiredSkill;
    public string[] PassiveStatType;
    public float[] PassiveValue;
    public string[] ModifierType;
    public float Duration;
    public float TimeInterval;
    public string TriggerCondition;
    public float TriggerValue;
    public float CoolDown;
    public int MaxStack;

    public List<StatModifier> GetStatModifiers()
    {
        List<StatModifier> modifiers = new List<StatModifier>();

        if (PassiveStatType == null || PassiveStatType.Length == 0)
            return modifiers;

        for (int i = 0; i < PassiveStatType.Length; i++)
        {
            if (!Enum.TryParse<StatType>(PassiveStatType[i], true, out StatType statType))
                continue;

            ModifierType modType = global::ModifierType.Flat;
            if (ModifierType != null && i < ModifierType.Length)
            {
                Enum.TryParse<ModifierType>(ModifierType[i], true, out modType);
            }

            if (PassiveValue != null && i < PassiveValue.Length && Mathf.Abs(PassiveValue[i]) > 0.0001f)
            {
                modifiers.Add(new StatModifier
                {
                    Type = statType,
                    ModType = modType,
                    Value = PassiveValue[i]
                });
            }
        }

        return modifiers;
    }
}
