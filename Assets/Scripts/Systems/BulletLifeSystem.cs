using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class BulletLifeSystem : SystemBase
{
    private BeginSimulationEntityCommandBufferSystem m_BeginSimEcb;
    //creating the Command buffer system variable

    protected override void OnCreate()
    {
        m_BeginSimEcb = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        //and a handle for the world Command Buffer System

    }
    protected override void OnUpdate()
    {
        var dt = Time.DeltaTime;
        //Getting time since last fame

        var commandBuffer = m_BeginSimEcb.CreateCommandBuffer().AsParallelWriter();
        //and a handle for the Command Buffer System

        Entities.
           ForEach((Entity entity, int nativeThreadIndex, ref LifeExpectancyData lifeExpectancyData) => {
           lifeExpectancyData.currentAge += dt;
               //adding time since last frame to the total time
           if (lifeExpectancyData.lifeExpectancy < lifeExpectancyData.currentAge)
            {
                commandBuffer.DestroyEntity(nativeThreadIndex, entity);
            }
           //and if the total time has passed the dessired life time we destroy the entity
        }).ScheduleParallel();

        m_BeginSimEcb.AddJobHandleForProducer(Dependency);
        //This will add our dependency to be played back on the BeginSimulationEntityCommandBuffer

    }
}
