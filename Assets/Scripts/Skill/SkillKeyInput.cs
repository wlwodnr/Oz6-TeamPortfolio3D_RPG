using System;
using UnityEngine;

public class SkillKeyInput : MonoBehaviour
{
    [SerializeField] private SkillExecutor skillExecutor;
    [SerializeField] private PlayerController playerController;

    private void Start()
    {
        if (skillExecutor == null) skillExecutor = GetComponent<SkillExecutor>();
        if (playerController == null) playerController = GetComponent<PlayerController>();
    }
    private void Update()
    {
        if (skillExecutor == null || GameDataManager.Instance == null || SkillTracker.Instance == null) return;

        CharacterMode currentMode = SkillTracker.Instance.SkillModel.CurrentMode;

        foreach (var pair in GameDataManager.Instance.ActiveSkillDataList)
        {
            ActiveSkillData data = pair.Value;

            if (string.IsNullOrEmpty(data.InputKey)) continue;

            if (Enum.TryParse(data.InputKey, true, out KeyCode targetKey))
            {
                if (Input.GetKeyDown(targetKey))
                {
                    string targetModeStr = string.IsNullOrEmpty(data.TargetMode) ? "Always" : data.TargetMode.Trim();

                    Debug.Log($"입력한 키: {targetKey} | 스킬ID: {data.Id} | 현재모드: {currentMode} | 스킬타겟모드: {targetModeStr}");

                    if (!targetModeStr.Equals("Always", StringComparison.OrdinalIgnoreCase) &&
                        !targetModeStr.Equals(currentMode.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (data.RequireAir && playerController != null && playerController._jumpCount == 0)
                    {
                        Debug.LogWarning($"{data.Name} 스킬은 공중에서만 사용할 수 있습니다!");
                        continue;
                    }

                    Debug.Log($"{data.Name} 스킬 실행.");

                    skillExecutor.TryExecuteSkill(data.Id);

                    break;
                }
            }
        }
    }
}