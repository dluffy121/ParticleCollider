using UnityEngine;
using Unity.Entities;

class SpawnerAuthoring : MonoBehaviour
{
    public GameObject Prefab;
    public Vector3[] SpawnRange;
    public Vector3[] SpawnImpulse;
    public int SpawnCount;
}

class SpawnerBaker : Baker<SpawnerAuthoring>
{
    public override void Bake(SpawnerAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new Spawner
        {
            // By default, each authoring GameObject turns into an Entity.
            // Given a GameObject (or authoring component), GetEntity looks up the resulting Entity.
            Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic),
            SpawnPosition = authoring.transform.position,
            MinSpawnRange = authoring.SpawnRange[0],
            MaxSpawnRange = authoring.SpawnRange[1],
            MinSpawnImpulse = authoring.SpawnImpulse[0],
            MaxSpawnImpulse = authoring.SpawnImpulse[1],
            SpawnCount = authoring.SpawnCount
        });
    }
}