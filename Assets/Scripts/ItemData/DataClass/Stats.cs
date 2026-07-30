using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Stats
{
    private Dictionary<StatType, float> _baseStats = new Dictionary<StatType, float>();

    private Dictionary<string, List<StatModifier>> _rawModifiers = new Dictionary<string, List<StatModifier>>();
    private Dictionary<string, int> _counts = new Dictionary<string, int>();

    private Dictionary<StatType, float> _flatCache = new Dictionary<StatType, float>();
    private Dictionary<StatType, float> _percentCache = new Dictionary<StatType, float>();

    public event Action<string> OnStatsUpdated;

    public Stats()
    {
        foreach(StatType type in Enum.GetValues(typeof(StatType)))
        {
            _baseStats[type] = 100;  // 테스트용 초기화
        }
    }

    public void InitializeBaseStats(PlayerStatData playerStatData)
    {
        if (playerStatData == null)
        {
            Debug.LogWarning("[Stats] PlayerStatData가 없어 기본 스탯을 초기화할 수 없습니다.");
            return;
        }

        foreach (StatType type in Enum.GetValues(typeof(StatType)))
        {
            _baseStats[type] = 0f;
        }

        _baseStats[StatType.AttackPower] = playerStatData.Atk;
        _baseStats[StatType.MaxHP] = playerStatData.HP;
        _baseStats[StatType.MaxMP] = playerStatData.MP;
        _baseStats[StatType.AttackSpeed] = playerStatData.AtkSpeed;
        NotifyAllStatsUpdated();
    }

    public void AddModifier(string itemId)  // item뿐 아니라 패시브 스킬도 따로 id를 만들어서 이걸로 추가하기
    {
        if (_rawModifiers.ContainsKey(itemId) == false)
        {
            var data = ItemDataBase.GetItemData(itemId) as StatUpItem;
            if (data == null)
            {
                var skillData = GameDataManager.Instance.GetPassiveSkillData(itemId);
                _rawModifiers.Add(itemId, skillData.GetStatModifiers());
                _counts.Add(itemId, 1);
            }
            else
            {
                var list = new List<StatModifier>();
                list.AddRange(data.StatModifiers);
                _rawModifiers.Add(itemId, list);
                _counts.Add(itemId, 1);
                Debug.Log($"{itemId} - {_counts[itemId]} 11");
            }

        }
        else
        {
            if (_counts.ContainsKey(itemId) == true)
            {
                _counts[itemId] = _counts[itemId] + 1;
                Debug.Log($"{itemId} - {_counts[itemId]} 22");
            }
        }

        UpdateCache();

        foreach (var list in _rawModifiers[itemId])
        {
            OnStatsUpdated?.Invoke(list.Type.ToString());
        }
    }
    public void RemoveModifier(string itemId)
    {
        if (_rawModifiers.ContainsKey(itemId) == false || _counts.ContainsKey(itemId) == false)
        {
            return;
        }

        List<StatModifier> statModifiers = _rawModifiers[itemId];
        int modifierCount = _counts[itemId] - 1;
        SetModifierCount(itemId, statModifiers, modifierCount);
        UpdateCache();
        NotifyModifierStatsUpdated(statModifiers); 
    }

    public void SetModifierCount(string modifierId, List<StatModifier> statModifiers, int modifierCount)
    {
        if (string.IsNullOrEmpty(modifierId) || statModifiers == null)
        {
            return;
        }

        if (modifierCount <= 0)
        {
            _counts.Remove(modifierId);
            _rawModifiers.Remove(modifierId);
        }
        else
        {
            _rawModifiers[modifierId] = statModifiers;
            _counts[modifierId] = modifierCount;
        }

        UpdateCache();
        NotifyModifierStatsUpdated(statModifiers);
    }

    private void UpdateCache()
    {
        _flatCache.Clear();
        _percentCache.Clear();

        foreach (var data in _counts)
        {
            string itemId = data.Key;
            int count = data.Value;
            if (count == 0) continue;

            foreach(StatModifier mod in _rawModifiers[itemId])
            {
                if (mod.ModType == ModifierType.Flat)
                    _flatCache[mod.Type] = _flatCache.GetValueOrDefault(mod.Type) + (mod.Value * count);
                else
                    _percentCache[mod.Type] = _percentCache.GetValueOrDefault(mod.Type) + (mod.Value * count);
            }
        }
        Debug.Log($"{_flatCache[StatType.AttackPower]}  12323");
    }

    public void ClearAllData() //초기화때만 사용
    {
        _rawModifiers.Clear();
        _counts.Clear();
        _flatCache.Clear();
        _percentCache.Clear();
    }

    public float GetValue(StatType type)
    {
        float flat = _flatCache.GetValueOrDefault(type, 0);
        float percent = _percentCache.GetValueOrDefault(type, 0);
        Debug.Log($"[Stats] - {type} flat - {flat}, percent - {percent}");
        return (_baseStats[type] + flat) * (1 + percent);
    }

    private void NotifyModifierStatsUpdated(List<StatModifier> statModifiers)
    {
        foreach (StatModifier statModifier in statModifiers)
        {
            OnStatsUpdated?.Invoke(statModifier.Type.ToString());
        }
    }

    private void NotifyAllStatsUpdated()
    {
        foreach (StatType statType in Enum.GetValues(typeof(StatType)))
        {
            OnStatsUpdated?.Invoke(statType.ToString());
        }
    }

}
