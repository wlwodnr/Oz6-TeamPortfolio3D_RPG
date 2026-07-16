using System;

public interface ISkillData
{
    object Id { get; }
    int RequiredLevel { get; }
    string[] RequiredSkill { get; }
}