using UnityEngine;

public class MonsterData
{
    public string Name;
    public string Description;
    public string PrefabPath;
    public string IconPath;

    public int BaseHp;
    public int BaseAttack;
    public float MoveSpeed;

    public float DetectRange;
    public float AttackRange;
    public float SpawnLimitRange;
    public float PatrolRange;        // 배회 반경
}
