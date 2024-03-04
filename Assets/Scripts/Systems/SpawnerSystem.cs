using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Mathematics;
using static Unity.Entities.SystemAPI;

[BurstCompile]
public partial struct SpawnerSystem : ISystem
{
    Random m_random;

    public void OnCreate(ref SystemState state)
    {
        m_random = new(1);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer.ParallelWriter ecb = GetEntityCommandBuffer(ref state);

        // Creates a new instance of the job, assigns the necessary data, and schedules the job in parallel.
        new ProcessSpawnerJob
        {
            Ecb = ecb,
            ElapsedTime = Time.ElapsedTime,
            random = new Random((uint)UnityEngine.Random.Range(1, 100000))
        }.ScheduleParallel();
    }

    private EntityCommandBuffer.ParallelWriter GetEntityCommandBuffer(ref SystemState state)
    {
        var ecbSingleton = GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        return ecb.AsParallelWriter();
    }

    [BurstCompile]
    public partial struct ProcessSpawnerJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter Ecb;
        public double ElapsedTime;
        public Random random;

        // IJobEntity generates a component data query based on the parameters of its `Execute` method.
        // This example queries for all Spawner components and uses `ref` to specify that the operation
        // requires read and write access. Unity processes `Execute` for each entity that matches the
        // component data query.
        private void Execute([ChunkIndexInQuery] int chunkIndex, ref Spawner spawner)
        {
            // If the next spawn time has passed.
            if (spawner.NextSpawnTime >= ElapsedTime)
                return;

            // Spawns a new entity and positions it at the spawner.
            Entity newEntity = Ecb.Instantiate(chunkIndex, spawner.Prefab);
            Ecb.AddComponent(chunkIndex, newEntity, new Particle() { Ready = true });

            var newPos = random.NextFloat3(spawner.MinSpawnRange, spawner.MaxSpawnRange);

            Ecb.SetComponent(chunkIndex, newEntity, LocalTransform.FromPosition(spawner.SpawnPosition + newPos));

            // Resets the next spawn time.
            spawner.NextSpawnTime = (float)ElapsedTime + spawner.SpawnRate;
        }
    }
}