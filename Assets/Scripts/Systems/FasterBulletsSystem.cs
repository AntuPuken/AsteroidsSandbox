using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class FasterBulletsSystem : SystemBase
{
    private BeginSimulationEntityCommandBufferSystem m_BeginSimECB;

    protected override void OnCreate()
    {
        m_BeginSimECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        Entities.
        WithAny <FasterBulletsPowerTag>().
        ForEach(( ref PlayerData playerData ) => {
            playerData.shootRechargeTime = 0.2f;
        }).ScheduleParallel();

        var commandBuffer = m_BeginSimECB.CreateCommandBuffer().AsParallelWriter();

        Entities.
         WithAny<RemoveFasterBulletsPowerTag>().
         ForEach((Entity entity, int nativeThreadIndex, ref PlayerData playerData) => {
             playerData.shootRechargeTime = 1;
             commandBuffer.RemoveComponent<RemoveFasterBulletsPowerTag>(nativeThreadIndex, entity);
             commandBuffer.RemoveComponent<FasterBulletsPowerTag>(nativeThreadIndex, entity);

         }).ScheduleParallel();
        m_BeginSimECB.AddJobHandleForProducer(this.Dependency);
    }
}
