using System;
using System.Collections.Generic;

[System.Serializable]
public class PassiveSkillData : GameDataBase, ISkillData
{
    public string Name;
    public string Description;
    public string IconPath;
    public string TargetMode;
    public int RequiredLevel;
    public string[] RequiredSkill;
    public List<StatModifier> StatModifiers;
    object ISkillData.Id => Id;
    int ISkillData.RequiredLevel => RequiredLevel;
    string[] ISkillData.RequiredSkill => RequiredSkill;
}
