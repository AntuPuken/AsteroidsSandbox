using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;
public class AsteroidTriggerSystem : JobComponentSystem
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
    struct AsteroidTriggerSystemJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<AsteroidTag> allAsteroids;
        [ReadOnly] public ComponentDataFromEntity<EnemyTag> allEnemies;
        [ReadOnly] public ComponentDataFromEntity<PlayerTag> allPlayers;
        [ReadOnly] public ComponentDataFromEntity<DestroyPlayerTag> allDestroyPlayers;
        [ReadOnly] public ComponentDataFromEntity<ShieldPowerTag> allShieldPowers;
    
        public EntityCommandBuffer entityCommandBuffer;

        public void Execute(TriggerEvent triggerEvent)
        {
         
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;
           
            if (allAsteroids.HasComponent(entityA) && allAsteroids.HasComponent(entityB))
            {
                return;
            }
            //PlayerAsteroidNoShield
            if (allAsteroids.HasComponent(entityA) && allPlayers.HasComponent(entityB) && !allShieldPowers.HasComponent(entityB) && !allDestroyPlayers.HasComponent(entityB))
            {
                      entityCommandBuffer.AddComponent(entityB, new DestroyPlayerTag { });
              
            }
        
            if (allAsteroids.HasComponent(entityB) && allPlayers.HasComponent(entityA) && !allShieldPowers.HasComponent(entityA) && !allDestroyPlayers.HasComponent(entityA))
            {
                     entityCommandBuffer.AddComponent(entityA, new DestroyPlayerTag { });
              

                }
        
            //PlayerAsteroidWithShield

            if (allAsteroids.HasComponent(entityA) && allPlayers.HasComponent(entityB) && allShieldPowers.HasComponent(entityB))
            {       
                entityCommandBuffer.AddComponent(entityA, new DestroyAsteroidTag { });

            }
            if (allAsteroids.HasComponent(entityB) && allPlayers.HasComponent(entityA) && allShieldPowers.HasComponent(entityA))
            {
                 entityCommandBuffer.AddComponent(entityB, new DestroyAsteroidTag { });

            }
            //EnemyAsteroid
            if (allEnemies.HasComponent(entityA) && allAsteroids.HasComponent(entityB))
            {
                 entityCommandBuffer.AddComponent(entityA, new EnemyDestroyTag { });
                entityCommandBuffer.AddComponent(entityB, new DestroyAsteroidTag { });


            }
            if (allEnemies.HasComponent(entityB) && allAsteroids.HasComponent(entityA))
            {
                entityCommandBuffer.AddComponent(entityA, new DestroyAsteroidTag { });
                entityCommandBuffer.AddComponent(entityB, new EnemyDestroyTag { });


            }
        }

    }
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {

        var job = new AsteroidTriggerSystemJob();
        job.allAsteroids = GetComponentDataFromEntity<AsteroidTag>(true);
        job.allPlayers = GetComponentDataFromEntity<PlayerTag>(true);
        job.allEnemies = GetComponentDataFromEntity<EnemyTag>(true);
        job.allShieldPowers = GetComponentDataFromEntity<ShieldPowerTag>(true);
        job.allDestroyPlayers = GetComponentDataFromEntity<DestroyPlayerTag>(true);
        job.entityCommandBuffer = commandBufferSystem.CreateCommandBuffer();
        JobHandle jobHandle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDependencies);
        commandBufferSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }

}
