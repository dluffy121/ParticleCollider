using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Mathematics;
using static Unity.Entities.SystemAPI;
using Unity.Jobs;
using Unity.Collections;

[BurstCompile]
public partial struct SpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer.ParallelWriter ecbpll = GetEntityCommandBufferParallel(ref state);

        new SpawnParticleJob()
        {
            ecb = ecbpll,
            spawner = GetSingleton<Spawner>(),
            random = new Random((uint)UnityEngine.Random.Range(1, 100000))
        }
        .Schedule(GetSingleton<Spawner>().SpawnCount, 1)
        .Complete();

        state.Enabled = false;
    }

    private EntityCommandBuffer.ParallelWriter GetEntityCommandBufferParallel(ref SystemState state)
    {
        var ecbSingleton = GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        return ecb.AsParallelWriter();
    }

    [BurstCompile]
    public partial struct SpawnParticleJob : IJobParallelFor
    {
        public EntityCommandBuffer.ParallelWriter ecb;
        [ReadOnly] public Spawner spawner;
        public Random random;

        public void Execute(int index)
        {
            Entity newParticle = ecb.Instantiate(index, spawner.Prefab);
            ecb.AddComponent(index, newParticle, new Particle() { Ready = true });

            var newPos = random.NextFloat3(spawner.MinSpawnRange, spawner.MaxSpawnRange);
            ecb.SetComponent(index, newParticle, LocalTransform.FromPosition(spawner.SpawnPosition + newPos));
        }
    }
}