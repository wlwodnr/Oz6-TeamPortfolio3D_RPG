using System.Collections.Generic;
using UnityEngine;

public class TestMonsterHudController : MonoBehaviour
{
    [SerializeField] private MonsterHudUI _monsterHudPrefab;
    [SerializeField] private Transform _hudSlotRoot;

    private Dictionary<int, MonsterHudUI> _activeHudList = new Dictionary<int, MonsterHudUI>();

    public MonsterHudUI AddMonsterHud(int instanceId, EnemyStatus targetStatus, Transform targetTransform)
    {
        if (_activeHudList.ContainsKey(instanceId))
        {
            return _activeHudList[instanceId];
        }

        if (_monsterHudPrefab == null || _hudSlotRoot == null || targetStatus == null)
        {
            return null;
        }

        MonsterModel model = new MonsterModel(targetStatus);
        MonsterViewModel viewModel = new MonsterViewModel(model);

        MonsterHudUI hudUI = Instantiate(_monsterHudPrefab, _hudSlotRoot);
        if (hudUI == null)
        {
            return null;
        }

        hudUI.OnHudClosed += HandleHudClosed;
        hudUI.BindViewModel(instanceId, viewModel, targetTransform);
        
        _activeHudList.Add(instanceId, hudUI);
        return hudUI;
    }

    private void HandleHudClosed(int instanceId)
    {
        RemoveMonsterHud(instanceId);
    }

    public void RemoveMonsterHud(int instanceId)
    {
        if (_activeHudList.TryGetValue(instanceId, out MonsterHudUI hudUI))
        {
            if (hudUI != null)
            {
                hudUI.OnHudClosed -= HandleHudClosed;
                Destroy(hudUI.gameObject);
            }
            _activeHudList.Remove(instanceId);
        }
    }

    public void RemoveAllMonsterHud()
    {
        foreach (var pair in _activeHudList)
        {
            if (pair.Value != null)
            {
                Destroy(pair.Value.gameObject);
            }
        }
        _activeHudList.Clear();
    }
}
