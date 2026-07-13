using System;
using System.Collections.Generic;
using UnityEngine;

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
            _baseStats[type] = 1;  // 테스트용 초기화
        }
    }

    public void AddModifier(string itemId)
    {
        if(_rawModifiers.ContainsKey(itemId) == false)
        {
            var data = ItemDataBase.GetItemData(itemId) as StatUpItem;
            _rawModifiers.Add(itemId, data.StatModifiers);
        }
        else
        {
            if(_counts.ContainsKey(itemId) == true)
            {
                _counts[itemId] = _counts[itemId] + 1;
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
        if(_rawModifiers.ContainsKey(itemId) == true)
        {
            _counts[itemId] = _counts[itemId] - 1;
        }

        UpdateCache();
        foreach (var list in _rawModifiers[itemId])
        {
            OnStatsUpdated?.Invoke(list.Type.ToString());
        }

        if (_counts[itemId] <= 0)
        {
            _counts.Remove(itemId);
            _rawModifiers.Remove(itemId);
        }

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
    }

    public float GetValue(StatType type)
    {
        float flat = _flatCache.GetValueOrDefault(type, 0);
        float percent = _percentCache.GetValueOrDefault(type, 0);
        return (_baseStats[type] + flat) * (1 + percent);
    }
}