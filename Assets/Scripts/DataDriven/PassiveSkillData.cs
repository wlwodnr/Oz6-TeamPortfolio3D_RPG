using System;
using System.Collections.Generic;

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
    public float[] PassiveModifier;
    public float Duration;
    public float TimeInterval;
    public string TriggerCondition;
    public float TriggerValue;
    public float CoolDown;
    public int MaxStack;
}
