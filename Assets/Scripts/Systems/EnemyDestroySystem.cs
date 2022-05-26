using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class EnemyDestroySystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    //creating the variable for the Command buffer system on end of simulation frame 

    protected override void OnCreate()
    {


          m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        //and a handle for the Command Buffer System

    }
    protected override void OnUpdate()
    {
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();
        // we create a new command buffer from the commmand buffer hanlde as a parralel writer


        Entities.
            WithAny<EnemyDestroyTag>().
            ForEach((Entity entity, int entityInQueryIndex) => {
                //on our command buffer we record destroy the enemy
                ecb.DestroyEntity(entityInQueryIndex, entity);
        }).ScheduleParallel();

        //This will add our dependency to be played back on the EndSimulationEntityCommandBuffer
        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);

    }

}
