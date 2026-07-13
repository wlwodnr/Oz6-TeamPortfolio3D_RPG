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

    public float DamageMultiplier;
    public List<float> MultiHitPercentList;

}