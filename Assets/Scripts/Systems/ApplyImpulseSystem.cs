using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Physics.Extensions;
using static Unity.Entities.SystemAPI;
using Unity.Collections;

// After the PhysicsInitializeGroup has finished, PhysicsWorld will be created.
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsInitializeGroup))]
[UpdateBefore(typeof(PhysicsSimulationGroup))]
public partial struct ApplyImpulseSystem : ISystem
{
    Random m_random;

    public void OnCreate(ref SystemState state)
    {
        m_random = new(1);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Random random = new Random((uint)UnityEngine.Random.Range(1, 100000));

        new ApplyImpulseJob()
        {
            spawner = GetSingleton<Spawner>(),
            random = random
        }.ScheduleParallel();
    }

    [BurstCompile]
    public partial struct ApplyImpulseJob : IJobEntity
    {
        [ReadOnly] public Spawner spawner;
        public Random random;

        private void Execute(RefRO<PhysicsMass> phyMassRef,
                             RefRO<LocalTransform> localTransformRef,
                             RefRW<Particle> particleRef,
                             RefRW<PhysicsVelocity> phyVelocityRef)
        {
            if (!particleRef.ValueRO.Ready)
                return;

            particleRef.ValueRW.Ready = false;

            var dir = random.NextFloat3(spawner.MinSpawnImpulse, spawner.MaxSpawnImpulse);
            phyVelocityRef.ValueRW.ApplyLinearImpulse(phyMassRef.ValueRO, localTransformRef.ValueRO.Scale, dir);
        }
    }
}
