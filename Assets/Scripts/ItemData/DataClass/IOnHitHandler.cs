public interface IOnHitHandler
{
    void Execute(PlayerModel player, IGameObjectEntity target, int effectValue, int stack);
}