using Unity.Entities;
using Unity.Mathematics;

public struct Spawner : IComponentData
{
    public Entity Prefab;
    public float3 SpawnPosition;
    public float3 MinSpawnRange;
    public float3 MaxSpawnRange;
    public int SpawnCount;

    public float3 MinSpawnImpulse;
    public float3 MaxSpawnImpulse;
}