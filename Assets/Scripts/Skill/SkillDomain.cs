using System;
public class SkillDomainService
{
public bool TryLearnSkill(SkillModel skillModel, ISkillData skillData, PlayerInfo playerInfo)
    {
        if (skillModel == null || skillData == null || playerInfo == null) return false;

        if (playerInfo.CurLevel < skillData.RequiredLevel) return false;

        if (skillData.RequiredSkill != null && skillData.RequiredSkill.Length > 0)
        {
            for (int i = 0; i < skillData.RequiredSkill.Length; i++)
            {
                if (skillModel.CheckSkillAdvance(skillData.RequiredSkill[i]) == false)
                {
                    return false;
                }
            }
        }

        if (playerInfo.SkillPoint < 1) return false;

        playerInfo.SkillPoint -= 1;
        skillModel.RegisterUnlockedSkill(skillData.Id.ToString());

        return true;
    }
}