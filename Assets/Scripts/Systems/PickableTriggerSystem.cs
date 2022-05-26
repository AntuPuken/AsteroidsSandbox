using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;
//[UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
public class PickableTriggerSystem : JobComponentSystem
{
    private BuildPhysicsWorld buildPhysicsWorld;
    private StepPhysicsWorld stepPhysicsWorld;
    private EndSimulationEntityCommandBufferSystem commandBufferSystem;
    protected override void OnCreate()
    {
        base.OnCreate();
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    //[BurstCompile]
    struct PickableTriggerSystemJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<ShieldTag> allShields;
        [ReadOnly] public ComponentDataFromEntity<PlayerTag> allPlayers;
        [ReadOnly] public ComponentDataFromEntity<FasterBulletsTag> allFasterBullets;
      public BufferFromEntity<PowerUpDataBuffer> lookup;

        public EntityCommandBuffer entityCommandBuffer;

        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            if (allShields.HasComponent(entityA) && allShields.HasComponent(entityB))
            {
                return;
            }
            //Shield pickable
            if (allShields.HasComponent(entityA) && allPlayers.HasComponent(entityB))
            {
                entityCommandBuffer.DestroyEntity(entityA);

                var buffer =lookup[entityB];
                buffer.Add(new PowerUpDataBuffer { PowerUpDataSet = new PowerUpData { type = 'S', time = 10, currentTime = 0 } });
                entityCommandBuffer.AddComponent(entityB, new ShieldPowerTag { });

            }
            if (allPlayers.HasComponent(entityA) && allShields.HasComponent(entityB))
            {
                entityCommandBuffer.DestroyEntity(entityB);
                var buffer = lookup[entityA];
                buffer.Add(new PowerUpDataBuffer{ PowerUpDataSet = new PowerUpData { type = 'S', time = 10, currentTime = 0 } });
                entityCommandBuffer.AddComponent(entityA, new ShieldPowerTag { });

            }
            //FasterBulletsPickable 
            if (allFasterBullets.HasComponent(entityA) && allPlayers.HasComponent(entityB))
            {
                entityCommandBuffer.DestroyEntity(entityA);
              
                   
                var buffer = lookup[entityB];
                    buffer.Add(new PowerUpDataBuffer { PowerUpDataSet = new PowerUpData { type = 'F', time = 10, currentTime = 0 } });
                    entityCommandBuffer.AddComponent(entityB, new FasterBulletsPowerTag { });
                

            }
            if (allPlayers.HasComponent(entityA) && allFasterBullets.HasComponent(entityB))
            {
                entityCommandBuffer.DestroyEntity(entityB);
                    var buffer = lookup[entityA];
                
                    buffer.Add(new PowerUpDataBuffer { PowerUpDataSet = new PowerUpData { type = 'F', time = 10, currentTime = 0 } });
                    entityCommandBuffer.AddComponent(entityA, new FasterBulletsPowerTag { });
              

            }


        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new PickableTriggerSystemJob();
        job.allShields = GetComponentDataFromEntity<ShieldTag>(true);
        job.allFasterBullets = GetComponentDataFromEntity<FasterBulletsTag>(true);
        job.lookup =  GetBufferFromEntity<PowerUpDataBuffer>();
        job.allPlayers = GetComponentDataFromEntity<PlayerTag>(true);
        job.entityCommandBuffer = commandBufferSystem.CreateCommandBuffer();
        JobHandle jobHandle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDependencies);
        commandBufferSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }
}
