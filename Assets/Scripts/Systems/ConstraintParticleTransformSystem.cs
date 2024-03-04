using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;

public partial struct ConstraintParticleTransformSystem// : ISystem
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