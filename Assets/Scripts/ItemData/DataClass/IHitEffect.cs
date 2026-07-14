using UnityEngine;

public interface IHitEffect
{
    int StackCount { get; set; }
    void OnHit(PlayerModel player, EnemyEntity target);
}

public class LifeStealEffect : IHitEffect
{
    public int Value;
    public int StackCount { get; set; }

    public LifeStealEffect(int value)
    {
        Value = value;
        StackCount = 1;
    }
    public void OnHit(PlayerModel player, EnemyEntity target)
    {
        //대충 플레이어 체력회복
    }
}