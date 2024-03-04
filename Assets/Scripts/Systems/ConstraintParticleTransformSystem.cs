using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Physics.Systems;

[UpdateAfter(typeof(PhysicsSimulationGroup))]
public partial struct ConstraintParticleTransformSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new ConstraintParticleTransformJob().ScheduleParallel();
    }

    [BurstCompile]
    public partial struct ConstraintParticleTransformJob : IJobEntity
    {
        private void Execute(RefRW<LocalTransform> localTransformRef, RefRO<Particle> particle)
        {
            localTransformRef.ValueRW.Position.y = 0;
        }
    }
}