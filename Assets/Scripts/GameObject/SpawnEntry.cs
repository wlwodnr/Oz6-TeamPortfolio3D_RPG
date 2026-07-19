using System;
using UnityEngine;

[Serializable]
public class SpawnEntry
{ 
    [Header("몬스터 프리팹")]
    [SerializeField] private GameObject Prefab_Monster;

    [Header("몬스터 데이터")]
    [SerializeField] private string _monsterDataId;

    [Header("생성 수")]
    [Min(1)]
    [SerializeField] private int _spawnCount;

    public GameObject MonsterPrefab
    {
        get 
        { 
            return Prefab_Monster; 
        }
    }

    public string MonsterDataId
    {
        get
        {
            return _monsterDataId;
        }
    }
    public int SpawnCount
    {
        get
        {
            return Mathf.Max(1, _spawnCount);
        }
    }

#if UNITY_EDITOR
    public void Validate()
    {
        if(_spawnCount < 1)
        {
            _spawnCount = 1;
        }
    }
#endif

}
