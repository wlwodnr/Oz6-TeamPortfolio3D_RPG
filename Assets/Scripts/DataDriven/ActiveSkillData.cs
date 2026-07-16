using System;
using System.Collections.Generic;

[System.Serializable]
public class ActiveSkillData : GameDataBase, ISkillData
{
    public string Name;
    public string Description;
    public string IconPath;
    public string TargetMode;
    public int RequiredLevel;
    public string[] RequiredSkill;
    public float DamageMultiplier;
    public List<float> MultiHitPercentList;
    public float CoolDown;
    public int Cost;
    public float CastTime;
    public float AttackRange;
    public int TargetCount;
    public string CrowdControl;
    object ISkillData.Id => Id;
    int ISkillData.RequiredLevel => RequiredLevel;
    string[] ISkillData.RequiredSkill => RequiredSkill;
}