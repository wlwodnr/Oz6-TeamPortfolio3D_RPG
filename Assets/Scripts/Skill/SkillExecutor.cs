using System;
using UnityEngine;

public class SkillExecutor : MonoBehaviour
{
    public void ExecuteSkill(string skillId)
    {
        ActiveSkillData activeSkill = GameDataManager.Instance.GetActiveSkillData(skillId);

        if (activeSkill == null)
        {
            return;
        }

        Debug.Log($"{activeSkill.Name} ¹ßµ¿");

    }

    public bool CheckSkillExecutable(string skillId)
    {
        ActiveSkillData activeSkill = GameDataManager.Instance.GetActiveSkillData(skillId);

        if (activeSkill == null)
        {
            return false;
        }

        return true;
    }
}