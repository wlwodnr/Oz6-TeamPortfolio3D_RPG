using UnityEngine;

public class LifeStealHandler : IOnHitHandler
{
    public void Execute(PlayerModel player, IGameObjectEntity target, int effectValue, int stack)
    {
        if (player == null) return;

        NetworkManager.Inst.LocalPlayerService.RequestChangePlayerHp(effectValue * stack);
        Debug.Log($"[LifeStealHandler] - {effectValue * stack} 회복 ");
    }
}
