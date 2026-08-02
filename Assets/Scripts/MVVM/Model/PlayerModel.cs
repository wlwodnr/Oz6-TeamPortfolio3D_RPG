using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerModel 
{
    private const string LevelUpModifierId = "player_level_up";

    private Stats _stats;
    private PlayerInfo _info;
    private readonly List<StatModifier> _levelUpStatModifiers = new List<StatModifier>();
    private int _lastProcessedLevel;

    public PlayerInfo Info => _info;
    public Stats Stats => _stats;
    // itemId - 갯수
    private Dictionary<string,int> _equipInventory = new Dictionary<string,int>();
    private HashSet<string> _learnedSkills = new HashSet<string>();

    //itemId - 데이터
    private Dictionary<string, int> _activeHitEffects = new Dictionary<string, int>();
    public IReadOnlyDictionary<string, int> ActiveHitEffects => _activeHitEffects;

    public event Action<string> OnPlayerStatsChanged;
    public event Action<string> OnPlayerInfoChanged;
    public event Action<string> OnSkillDataChanged;

    public PlayerModel()
    {
        _info = new PlayerInfo();
        _stats = new Stats();
        _stats.OnStatsUpdated += HandleStatsUpdated;
        _info.OnInfoChanged += HandleInfoUpdated;
        _lastProcessedLevel = _info.CurLevel;

        _levelUpStatModifiers.Add(new StatModifier { Type = StatType.AttackPower, ModType = ModifierType.Flat, Value = 3f });
        _levelUpStatModifiers.Add(new StatModifier { Type = StatType.MaxHP, ModType = ModifierType.Flat, Value = 30f });
        _levelUpStatModifiers.Add(new StatModifier { Type = StatType.MaxMP, ModType = ModifierType.Flat, Value = 10f });

        _info.Coins = 10000;
        _learnedSkills = new HashSet<string>();  // 신규 - 스킬 테스트용 액티브 스킬 습득

        LearnActive("Active_H_01");
        LearnActive("Active_H_02");
        LearnActive("Active_B_01");
        LearnActive("Active_B_02");
        LearnActive("Active_03");

    }

    public void InitializeStats(PlayerStatData playerStatData)
    {
        _stats.InitializeBaseStats(playerStatData);
        ApplyLevelUpStatModifiers();

        if (_info.CurHp <= 0f)
        {
            _info.CurHp = GetStatValue(StatType.MaxHP);
        }

        if (_info.CurMp <= 0f)
        {
            _info.CurMp = GetStatValue(StatType.MaxMP);
        }
    }

    public void LoadPlayerInfo(PlayerSaveData playerSaveData)
    {
        if (playerSaveData == null) return;

        _info.Name = playerSaveData.Name;
        _info.CurLevel = playerSaveData.CurLevel;
        _info.TotalExp = playerSaveData.TotalExp;
        _info.CurHp = playerSaveData.CurHp;
        _info.CurMp= playerSaveData.CurMp;
        _info.Coins= playerSaveData.Coins;
    }

    public void LoadSkillData(SkillSaveData skillSaveData)
    {
        if(skillSaveData == null) return;

        LearnedActiveSkill.Clear();
        LearnedPassiveSkill.Clear();

        LearnedActiveSkill.AddRange(skillSaveData.LearnedActiveSkills);
        LearnedPassiveSkill.AddRange(skillSaveData.LearnedPassiveSkills);
    }

    public void Additem(string itemId)
    {
        var itemData = ItemDataBase.GetItemData(itemId);
        if(itemData is StatUpItem statItem)
        {
            _stats.AddModifier(itemId);
            AddEquipInventory(itemId);
        }
        else if(itemData is IEquipable equipable)
        {
            if (_activeHitEffects.ContainsKey(itemId))
            {
                _activeHitEffects[itemId] += 1;
                AddEquipInventory(itemId);
            }
            else
            {
                _activeHitEffects.Add(itemId, 1);
                AddEquipInventory(itemId);
            }
        }
        else
        {
            return;
        }
    }

    public void RemoveItem(string itemId)
    {
        var itemData = ItemDataBase.GetItemData(itemId);
        if (itemData is IEquipable equipable)
        {
            switch (equipable.EffectType)
            {
                case EffectType.StatUp:
                    _stats.RemoveModifier(itemId);
                    RemoveEquipInventory(itemId);
                    break;
                default:
                    _activeHitEffects.Remove(itemId);
                    RemoveEquipInventory(itemId);
                    break;
            }
        }
        else
        {
            return;
        }
    }

    public void AddEquipInventory(string itemId)
    {
        if(_equipInventory.ContainsKey(itemId) == true)
        {
            _equipInventory[itemId] = _equipInventory[itemId] + 1;
        }
        else
        {
            _equipInventory.Add(itemId, 1);
        }
    }

    public void RemoveEquipInventory(string itemId)
    {
        if(_equipInventory.ContainsKey(itemId) == true)
        {
            _equipInventory[itemId] = _equipInventory[itemId] - 1;
        }

        if (_equipInventory[itemId] <= 0)
        {
            _equipInventory.Remove(itemId);
        }
    }



    private void HandleStatsUpdated(string changedType)
    {
        OnPlayerStatsChanged?.Invoke(changedType);
    }

    private void HandleInfoUpdated(string changedType)
    {
        if (changedType == nameof(PlayerInfo.CurLevel))
        {
            int currentLevel = _info.CurLevel;
            bool didLevelUp = currentLevel > _lastProcessedLevel;

            ApplyLevelUpStatModifiers();
            _lastProcessedLevel = currentLevel;

            if (didLevelUp == true)
            {
                RestoreHealthAndManaToMaximum();
            }
        }

        OnPlayerInfoChanged?.Invoke(changedType);
    }

    private void ApplyLevelUpStatModifiers()
    {
        int levelUpCount = Mathf.Max(0, _info.CurLevel - 1);
        _stats.SetModifierCount(LevelUpModifierId, _levelUpStatModifiers, levelUpCount);
    }

    private void RestoreHealthAndManaToMaximum()
    {
        _info.CurHp = GetStatValue(StatType.MaxHP);
        _info.CurMp = GetStatValue(StatType.MaxMP);
    }

    public void AddExperience(float experience)
    {
        if (experience <= 0f)
        {
            return;
        }

        _info.TotalExp += experience;
    }

    public void AddGold(int gold)
    {
        if (gold <= 0)
        {
            return;
        }

        _info.Coins += gold;
    }

    public float GetStatValue(StatType statType)
    {
        return _stats.GetValue(statType);
    }

    public float GetRequiredTotalExperienceForNextLevel()
    {
        return PlayerLevelProgression.GetRequiredTotalExperienceForNextLevel(_info.CurLevel);
    }

    public PlayerSaveData CaptureData()
    {
        PlayerSaveData data = new PlayerSaveData()
        {
            Name = _info.Name,
            CurLevel = _info.CurLevel,
            TotalExp = _info.TotalExp,
            SkillPoint = _info.SkillPoint,
            CurHp = _info.CurHp,
            CurMp = _info.CurMp,
            Coins = _info.Coins
        };
        
        return data;
    }
    // 최대 스탯 오버 방지
    public void ChangeHp(float amount)
    {
        Info.CurHp += amount;
        float maxHp = GetStatValue(StatType.MaxHP);

        if (Info.CurHp > maxHp)
        {
            Info.CurHp = maxHp;
        }

        if (Info.CurHp < 0f)
        {
            Info.CurHp = 0f;
        }

        OnPlayerInfoChanged?.Invoke(nameof(PlayerInfo.CurHp));
    }

    public void ChangeMp(float amount)
    {
        Info.CurMp += amount;
        float maxMp = GetStatValue(StatType.MaxMP);

        if (Info.CurMp > maxMp)
        {
            Info.CurMp = maxMp;
        }

        if (Info.CurMp < 0f)
        {
            Info.CurMp = 0f;
        }

        OnPlayerInfoChanged?.Invoke(nameof(PlayerInfo.CurMp));
    }

    // 아래는 임시

    public HashSet<string> LearnedPassiveSkill = new HashSet<string>();
    public HashSet<string> LearnedActiveSkill = new HashSet<string>();

    public void LearnPassive(string id)
    {
        if (LearnedPassiveSkill.Contains(id) == false)
        {
            LearnedPassiveSkill.Add(id);
            _learnedSkills.Add(id); // 신규 - 스킬아이디 해시셋 등록
            _stats.AddModifier(id); // 신규 - 패시브로 증가한 스탯 연동
            OnSkillDataChanged?.Invoke(id);  // 신규 - 이벤트 발생 알림
        }
    }

    public void LearnActive(string id)
    {
        if (LearnedActiveSkill.Contains(id) == false)
        {
            LearnedActiveSkill.Add(id);
            _learnedSkills.Add(id); // 신규 - 스킬아이디 해시셋 등록
            OnSkillDataChanged?.Invoke(id);  // 신규 - 이벤트 발생 알림
        }
    }
    public bool HasLearnedPassive(string id)
    {
        if (LearnedPassiveSkill.Contains(id) == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool HasLearnedActive(string id)
    {
        if (LearnedActiveSkill.Contains(id) == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public SkillSaveData CaptureSkillData()
    {
        SkillSaveData skillSaveData = new SkillSaveData();

        skillSaveData.LearnedActiveSkills.AddRange(LearnedActiveSkill);
        skillSaveData.LearnedPassiveSkills.AddRange(LearnedPassiveSkill);

        return skillSaveData;
    }
}
