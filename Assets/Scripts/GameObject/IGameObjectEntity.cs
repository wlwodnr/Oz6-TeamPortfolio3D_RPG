public interface IGameObjectEntity
{
    int InstanceId { get; }

    void InitEntity(int instanceId, string dataId);

    void ResetEntity();
}