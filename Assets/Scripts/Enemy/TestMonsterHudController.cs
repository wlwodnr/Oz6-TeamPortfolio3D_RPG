using System.Collections.Generic;
using UnityEngine;

public class MonsterHudController : MonoBehaviour
{
    [SerializeField] private GameObject _monsterHudPrefab;
    [SerializeField] private Transform _hudSlotRoot;

    private Dictionary<int, MonsterHudUI> _activeHudList = new Dictionary<int, MonsterHudUI>();

    public MonsterHudUI AddMonsterHud(int instanceId, MonsterViewModel viewModel, Transform targetTransform)
    {
        if (_activeHudList.ContainsKey(instanceId))
        {
            return _activeHudList[instanceId];
        }

        if (_monsterHudPrefab == null || _hudSlotRoot == null)
        {
            return null;
        }

        GameObject hudObj = Instantiate(_monsterHudPrefab, _hudSlotRoot);
        MonsterHudUI hudUI = hudObj.GetComponent<MonsterHudUI>();

        if (hudUI == null)
        {
            Destroy(hudObj);
            return null;
        }

        hudUI.BindViewModel(viewModel, targetTransform);
        _activeHudList.Add(instanceId, hudUI);

        return hudUI;
    }

    public void RemoveMonsterHud(int instanceId)
    {
        if (_activeHudList.TryGetValue(instanceId, out MonsterHudUI hudUI))
        {
            if (hudUI != null)
            {
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
