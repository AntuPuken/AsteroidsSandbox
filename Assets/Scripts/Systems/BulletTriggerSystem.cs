using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;
public class BulletTriggerSystem : JobComponentSystem
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
    struct BulletTriggerSystemJob : ITriggerEventsJob
    {
       [ReadOnly] public ComponentDataFromEntity<BulletTag> allBullets;
        [ReadOnly] public ComponentDataFromEntity<AsteroidTag> allAsteroids;
        [ReadOnly] public ComponentDataFromEntity<EnemyTag> allEnemies;
        [ReadOnly] public ComponentDataFromEntity<PlayerTag> allPlayers;
        [ReadOnly] public ComponentDataFromEntity<EnemyBulletTag> allEnemyBullets;
        [ReadOnly] public ComponentDataFromEntity<PlayerBulletTag> allPlayerBullets;
        [ReadOnly] public ComponentDataFromEntity<ShieldPowerTag> allShieldPowers;

        public EntityCommandBuffer entityCommandBuffer;

        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;
          
            if (allBullets.HasComponent(entityA) && allBullets.HasComponent(entityB))
            {
                return;
            }      
            //BUlletsAsteroids
            if (allBullets.HasComponent(entityA) && allAsteroids.HasComponent(entityB))
            {
                 entityCommandBuffer.DestroyEntity(entityA);
                entityCommandBuffer.AddComponent(entityB, new DestroyAsteroidTag { });
              //  entityCommandBuffer.AddComponent(entityB, new DestroyAsteroidTag { });

            }
            if (allAsteroids.HasComponent(entityA) && allBullets.HasComponent(entityB))
            {
                entityCommandBuffer.DestroyEntity(entityB);
                entityCommandBuffer.AddComponent(entityA, new DestroyAsteroidTag { });
            }
            //PlayerBullet destroy enemy
           
            if (allEnemies.HasComponent(entityA) && allPlayerBullets.HasComponent(entityB) )
            {
                //entityCommandBuffer.DestroyEntity(entityA);
                entityCommandBuffer.AddComponent(entityA, new EnemyDestroyTag { });
                entityCommandBuffer.DestroyEntity(entityB);
           
            }
            if (allPlayerBullets.HasComponent(entityA) && allEnemies.HasComponent(entityB))
            {
                entityCommandBuffer.DestroyEntity(entityA);
                //entityCommandBuffer.DestroyEntity(entityB);
                entityCommandBuffer.AddComponent(entityB, new EnemyDestroyTag { });
            }
            //EnemyBulletDestroyPlayer
            if (allPlayers.HasComponent(entityA) && allEnemyBullets.HasComponent(entityB))
            {
                if (!allShieldPowers.HasComponent(entityA)) { entityCommandBuffer.AddComponent(entityA, new DestroyPlayerTag { }); }
                entityCommandBuffer.DestroyEntity(entityB);

            }
            if (allEnemyBullets.HasComponent(entityA) && allPlayers.HasComponent(entityB))
            {
                entityCommandBuffer.DestroyEntity(entityA);
                if (!allShieldPowers.HasComponent(entityB)) { entityCommandBuffer.AddComponent(entityB, new DestroyPlayerTag { }); }

               
            }

        }

    }
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new BulletTriggerSystemJob();
        job.allAsteroids = GetComponentDataFromEntity<AsteroidTag>(true);
        job.allBullets = GetComponentDataFromEntity<BulletTag>(true);
        job.allPlayers = GetComponentDataFromEntity<PlayerTag>(true);
        job.allShieldPowers = GetComponentDataFromEntity<ShieldPowerTag>(true);

        job.allEnemyBullets = GetComponentDataFromEntity<EnemyBulletTag>(true);
        job.allPlayerBullets = GetComponentDataFromEntity<PlayerBulletTag>(true);
        
        job.allEnemies = GetComponentDataFromEntity<EnemyTag>(true);
        job.entityCommandBuffer = commandBufferSystem.CreateCommandBuffer();
        JobHandle jobHandle = job.Schedule( stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDependencies);
        //jobHandle.Complete();
        commandBufferSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }
}
