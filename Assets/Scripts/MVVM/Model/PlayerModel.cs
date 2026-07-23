using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel 
{
    private Stats _stats;
    private PlayerInfo _info;

    public PlayerInfo Info => _info;
    // itemId - 갯수
    private Dictionary<string,int> _inventory = new Dictionary<string, int>();    
    private Dictionary<string,int> _equipInventory = new Dictionary<string,int>();

    private HashSet<string> _learnedSkills = new HashSet<string>();

    //itemId - 데이터
    private Dictionary<string, IHitEffect> _activeHitEffects = new Dictionary<string, IHitEffect>();

    public event Action<string> OnPlayerStatsChanged;
    public event Action<string> OnPlayerInfoChanged;

    public PlayerModel()
    {
        _info = new PlayerInfo();
        _stats = new Stats();
        _stats.OnStatsUpdated += HandleStatsUpdated;
        _info.OnInfoChanged += HandleInfoUpdated;
        _learnedSkills = new HashSet<string>();

        LearnActive("Active_H_01");
        LearnActive("Active_H_02");
        LearnActive("Active_B_01");
        LearnActive("Active_B_02");
        LearnActive("Active_03");
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
                _activeHitEffects[itemId].StackCount += 1;
                AddEquipInventory(itemId);
            }
            else
            {
                var effect = ItemEffectFactory.Create(itemData, equipable.EffectType);
                if (effect != null)
                {
                    _activeHitEffects.Add(itemId, effect);
                    AddEquipInventory(itemId);
                }
            }
        }
        else
        {
            AddInventory(itemId);
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
                case EffectType.LifeSteal:
                    var effect = ItemEffectFactory.Create(itemData, equipable.EffectType);
                    if (effect != null)
                    {
                        _activeHitEffects.Add(itemId, effect);
                        RemoveEquipInventory(itemId);
                    }
                    break;
            }
        }
        else
        {
            AddInventory(itemId);
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
        // 여기서 장비템 정보창 MVVM 구조 VM 정보전달 메서드 실행
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
        // 여기서 장비템 정보창 MVVM 구조 VM 정보전달 메서드 실행
    }

    public void AddInventory(string itemId)
    {
        if (_inventory.ContainsKey(itemId) == true)
        {
            _inventory[itemId] = _inventory[itemId] + 1;
        }
        else
        {
            _inventory.Add(itemId, 1);
        }
        // 여기서 인벤토리 MVVM 구조 VM 정보전달 메서드 실행
    }

    public void RemoveInventory(string itemId)
    {
        if (_inventory.ContainsKey(itemId) == true)
        {
            _inventory[itemId] = _inventory[itemId] - 1;
        }

        if (_inventory[itemId] <= 0)
        {
            _inventory.Remove(itemId);
        }
        // 여기서 인벤토리 MVVM 구조 VM 정보전달 메서드 실행
    }

    private void HandleStatsUpdated(string changedType)
    {
        OnPlayerStatsChanged?.Invoke(changedType);
    }

    private void HandleInfoUpdated(string changedType)
    {
        OnPlayerInfoChanged?.Invoke(changedType);
    }

    public float GetStatValue(StatType statType)
    {
        return _stats.GetValue(statType);
    }


    // 아래는 스킬 관련 데이터/메서드 (수정될 확률 높음)

    public HashSet<string> LearnedPassiveSkill = new HashSet<string>();
    public HashSet<string> LearnedActiveSkill = new HashSet<string>();

    public void LearnPassive(string id)
    {
        if (LearnedPassiveSkill.Contains(id) == false)
        {
            LearnedPassiveSkill.Add(id);
        }
    }

    public void LearnActive(string id)
    {
        if (LearnedActiveSkill.Contains(id) == false)
        {
            LearnedActiveSkill.Add(id);
        }
    }
    public bool HasLearnedPassive(string id)
    {
        if(LearnedPassiveSkill.Contains(id) == true)
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
}
