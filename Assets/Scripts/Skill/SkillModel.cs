using System.Collections.Generic;
using UnityEngine;

public enum CharacterMode { Hunt, Boss }

public class SkillModel
{
    private CharacterMode _currentMode = CharacterMode.Hunt;
    private readonly Dictionary<string, float> _remainCoolTimes = new();

    private readonly List<string> _coolTimeKeysCache = new();

    public CharacterMode CurrentMode
    {
        get => _currentMode;
        set => _currentMode = value;
    }

    public bool IsSkillReady(string skillId)
    {
        if (_remainCoolTimes.TryGetValue(skillId, out float remain))
        {
            return remain <= 0f;
        }
        return true;
    }

    public float GetRemainCoolTime(string skillId)
    {
        return _remainCoolTimes.GetValueOrDefault(skillId, 0f);
    }

    public void StartCoolTime(string skillId, float duration)
    {
        if (duration <= 0f) return;
        _remainCoolTimes[skillId] = duration;
    }

    public void UpdateCoolTimes(float deltaTime)
    {
        if (_remainCoolTimes.Count == 0) return;

        _coolTimeKeysCache.Clear();
        _coolTimeKeysCache.AddRange(_remainCoolTimes.Keys);

        for (int i = 0; i < _coolTimeKeysCache.Count; i++)
        {
            string key = _coolTimeKeysCache[i];
            if (_remainCoolTimes[key] > 0f)
            {
                _remainCoolTimes[key] -= deltaTime;
                if (_remainCoolTimes[key] < 0f) _remainCoolTimes[key] = 0f;
            }
        }
    }
}