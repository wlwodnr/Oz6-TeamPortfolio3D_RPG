using System;
using System.Collections.Generic;

public class SkillModel
{
    public bool IsBossModeActive { get; private set; } = false;     // false : 사냥 모드

    public HashSet<string> UnlockedSkillIds { get; private set; } = new HashSet<string>();

    public void ChangeAttackMode()
    {
        IsBossModeActive = !IsBossModeActive;
    }

    public bool CheckSkillAdvance(string targetSkillId)
    {
        return UnlockedSkillIds.Contains(targetSkillId);
    }

    public void RegisterUnlockedSkill(string skillId)
    {
        UnlockedSkillIds.Add(skillId);
    }

    public string GetCurrentSkillId(string baseSkillId)
    {
        if (baseSkillId == "Active_H_01" || baseSkillId == "Active_B_01")
        {
            bool hasAdvancedSkill = CheckSkillAdvance("Active_H_02");

            if (hasAdvancedSkill)
            {
                if (IsBossModeActive == true)
                {
                    return "Active_B_02";
                }

                return "Active_H_02";
            }

            if (IsBossModeActive == true)
            {
                return "Active_B_01";
            }

            return "Active_H_01";
        }

        return baseSkillId;
    }
}