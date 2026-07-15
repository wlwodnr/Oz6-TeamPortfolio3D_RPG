using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;


public class SkillExecutor
{

    public async UniTask ExecuteSkill(string characterId, string baseSkillId, SkillModel skillModel)
    {
        string executableSkillId = skillModel.GetCurrentSkillId(baseSkillId);

        CharacterData character = GameDataManager.Instance.GetCharacterData(characterId);
        SkillData skill = GameDataManager.Instance.GetSkill(executableSkillId);

        if (character == null || skill == null) return;

        float skillBasePower = character.Atk * skill.DamageMultiplier;

        Debug.Log($"{skill.Name} 발동.");

        // (신규) 엑셀에서 넘어온 타수 배열(MultiHitPercentList) 개수만큼 가비지 없이 비동기 루프 구동
        for (int i = 0; i < skill.MultiHitPercentList.Count; i++)
        {
            float currentHitPercent = skill.MultiHitPercentList[i];     // 현재 순서의 다단 히트 개별 배율 데이터 할당
            float finalDamage = skillBasePower * currentHitPercent;     // 기본 위력에 타수 배율을 최종 적용하여 피해량 도출

            Debug.Log($"{skill.Name}의 최종 대미지 : {finalDamage}");     // 5칸 띄고 주석: 타수별 최종 정산 결과 로그 출력

            // (팩트) WaitForSeconds와 달리 구조체(Value Type) 기반으로 구동되어 힙 메모리 찌꺼기를 일절 남기지 않음
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f));     // (신규) 게임 화면이 얼어붙지 않도록 프레임을 유지하며 0.2초간 비동기 대기 가동
        }
    }

    public async UniTask ExecuteModeChangeSkill(SkillModel skillModel, string toggleSkillId)
    {
        SkillData toggleSkillData = GameDataManager.Instance.GetSkill(toggleSkillId);

        if (toggleSkillData == null || skillModel == null) return;

        await UniTask.Delay(TimeSpan.FromSeconds(0.15f));     // (신규) 상태 스위칭 직전 버프 연출 시간 확보를 위한 지연 기능 소모

        skillModel.ChangeAttackMode();

        string currentModeName = skillModel.IsBossModeActive ? "보스 모드 (단일 특화)" : "사냥 모드 (광역 기본)";
        Debug.Log($"모드 전환 - {toggleSkillData.Name}의 현재 상태 : {currentModeName}");
    }
}