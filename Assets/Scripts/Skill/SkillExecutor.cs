using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillExecutor
{
    public IEnumerator ExecuteSkillCoroutine(string characterId, string baseSkillId, SkillModel skillModel)
    {
        string executableSkillId = skillModel.GetCurrentSkillId(baseSkillId);

        CharacterData character = GameDataManager.Instance.GetCharacterData(characterId);
        SkillData skill = GameDataManager.Instance.GetSkill(executableSkillId);

        if (character == null || skill == null) yield break;

        float skillBasePower = character.Atk * skill.DamageMultiplier;

        Debug.Log($"{skill.Name} 발동.");

        for (int i = 0; i < skill.MultiHitPercentList.Count; i++)
        {
            float currentHitPercent = skill.MultiHitPercentList[i];
            float finalDamage = skillBasePower * currentHitPercent;

            Debug.Log($"{skill.Name}의 최종 대미지 : {finalDamage}");

            yield return new WaitForSeconds(0.2f);
        }
    }

    public IEnumerator ExecuteModeChangeCoroutine(SkillModel skillModel, string toggleSkillId)
    {
        SkillData toggleSkillData = GameDataManager.Instance.GetSkill(toggleSkillId);

        if (toggleSkillData == null || skillModel == null) yield break;

        yield return new WaitForSeconds(0.15f);

        skillModel.ChangeAttackMode();

        string currentModeName = skillModel.IsBossModeActive ? "보스 모드 (단일 특화)" : "사냥 모드 (광역 기본)";
        Debug.Log($"모드 전환 - {toggleSkillData.Name}의 현재 상태 : {currentModeName}");
    }
}