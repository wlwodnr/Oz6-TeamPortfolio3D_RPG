using System;
using System.Collections.Generic;


[System.Serializable]
public class CharacterData : GameDataBase
{
    public string Name;
    public string SkillList;

    public int Atk;
}

[System.Serializable]
public class SkillData : GameDataBase
{
    public string Name;
    public string Description;
    public string IconPath;
    public string SkillType;
    public string TargetMode;

    public float DamageMultiplier;
    public List<float> MultiHitPercentList;

    public int RequiredLevel;
}