using UnityEngine;

public class SkillTracker : MonoBehaviour
{
    public static SkillTracker Instance { get; private set; }

    private PlayerModel _playerModel;
    private SkillModel _skillModel;

    public SkillModel SkillModel => _skillModel ??= new SkillModel();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _skillModel ??= new SkillModel();
        }
        else
        {
            Debug.LogWarning($"중복된 SkillTracker가 발견되어 파괴합니다.");
            Destroy(gameObject);
        }
    }

    public void Init(PlayerModel playerModel)
    {
        if (playerModel == null)
        {
            Debug.LogWarning($"PlayerModel이 존재하지 않습니다.");
            return;
        }

        _playerModel = playerModel;

        _skillModel ??= new SkillModel();

        RefreshModePassives();
    }

    private void Update()
    {
        _skillModel?.UpdateCoolTimes(Time.deltaTime);
    }

    public bool TryLearnActiveSkill(string skillId)
    {
        if (GameDataManager.Instance == null || _playerModel == null) return false;

        ActiveSkillData data = GameDataManager.Instance.GetActiveSkillData(skillId);
        if (data == null)
        {
            Debug.LogWarning($"액티브 스킬 {skillId}의 데이터를 찾을 수 없습니다.");
            return false;
        }

        if (_playerModel.HasLearnedActive(skillId)) return false;

        if (_playerModel.Info.CurLevel < data.RequiredLevel)
        {
            Debug.LogWarning($"레벨이 부족하여 {skillId} 스킬을 습득할 수 없습니다. 요구 레벨: {data.RequiredLevel}");
            return false;
        }

        if (data.RequiredSkill != null && data.RequiredSkill.Length > 0)
        {
            foreach (string reqSkillId in data.RequiredSkill)
            {
                if (!_playerModel.HasLearnedActive(reqSkillId))
                {
                    Debug.LogWarning($"선행 스킬을 습득하지 않아 {skillId} 스킬을 습득할 수 없습니다. 요구 선행 스킬: {reqSkillId}");
                    return false;
                }
            }
        }

        _playerModel.LearnActive(skillId);
        Debug.Log($"{skillId} 스킬이 정상적으로 습득된 상태입니다.");
        return true;
    }

    public bool TryLearnPassiveSkill(string skillId)
    {
        if (GameDataManager.Instance == null || _playerModel == null) return false;

        PassiveSkillData data = GameDataManager.Instance.GetPassiveSkillData(skillId);
        if (data == null)
        {
            Debug.LogWarning($"패시브 스킬 {skillId}의 데이터를 찾을 수 없습니다.");
            return false;
        }

        if (_playerModel.HasLearnedPassive(skillId)) return false;

        if (_playerModel.Info.CurLevel < data.RequiredLevel)
        {
            Debug.LogWarning($"레벨이 부족하여 {skillId} 스킬을 습득할 수 없습니다. 요구 레벨: {data.RequiredLevel}");
            return false;
        }

        if (data.RequiredSkill != null && data.RequiredSkill.Length > 0)
        {
            foreach (string reqSkillId in data.RequiredSkill)
            {
                bool hasReq = _playerModel.HasLearnedActive(reqSkillId) || _playerModel.HasLearnedPassive(reqSkillId);
                if (!hasReq)
                {
                    Debug.LogWarning($"선행 스킬을 습득하지 않아 {skillId} 스킬을 습득할 수 없습니다. 요구 선행 스킬: {reqSkillId}");
                    return false;
                }
            }
        }

        _playerModel.LearnPassive(skillId);
        Debug.Log($"{skillId} 스킬이 정상적으로 습득된 상태입니다.");

        RefreshModePassives();
        return true;
    }

    public void ChangeMode(CharacterMode newMode)
    {
        if (_skillModel == null || _skillModel.CurrentMode == newMode) return;

        RemoveCurrentModePassive();

        _skillModel.CurrentMode = newMode;
        Debug.Log($"{newMode}로 모드를 전환합니다.");

        RefreshModePassives();
    }

    private void RefreshModePassives()
    {
        if (_playerModel == null || GameDataManager.Instance == null) return;

        foreach (string passiveId in _playerModel.LearnedPassiveSkill)
        {
            PassiveSkillData data = GameDataManager.Instance.GetPassiveSkillData(passiveId);
            if (data == null)
            {
                Debug.LogWarning($"{passiveId}의 데이터를 찾을 수 없습니다.");
                continue;
            }

            if (data.TargetMode == "Always" || data.TargetMode == _skillModel.CurrentMode.ToString())
            {
                _playerModel.Additem(passiveId);
            }
        }
    }

    private void RemoveCurrentModePassive()
    {
        if (_playerModel == null || GameDataManager.Instance == null) return;

        foreach (string passiveId in _playerModel.LearnedPassiveSkill)
        {
            PassiveSkillData data = GameDataManager.Instance.GetPassiveSkillData(passiveId);
            if (data == null) continue;

            if (data.TargetMode == _skillModel.CurrentMode.ToString() && data.TargetMode != "Always")
            {
                _playerModel.RemoveItem(passiveId);
            }
        }
    }
}