using System;

[System.Serializable]
public class PassiveSkillData : GameDataBase, ISkillData
{
    public string Name;
    public string Description;
    public string IconPath;
    public string TargetMode;
    public int RequiredLevel;
    public string[] RequiredSkill;
    public string PassiveStatType;
    public float PassiveValue;
    object ISkillData.Id => Id;
    int ISkillData.RequiredLevel => RequiredLevel;
    string[] ISkillData.RequiredSkill => RequiredSkill;
}
