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

    public string GetCurrentSkillId(string baseSkillId)
    {
        if (baseSkillId == "Active_01" && IsBossModeActive == true)
        {
            return "Active_02";
        }

        return baseSkillId;
    }
}