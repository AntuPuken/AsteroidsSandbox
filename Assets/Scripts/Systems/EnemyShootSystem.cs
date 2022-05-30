using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class EnemyShootSystem : SystemBase
{
    private Entity bulletPrefab;
    private BeginSimulationEntityCommandBufferSystem m_BeginSimECB;
    //creating the Command buffer system variable

    protected override void OnCreate()
    {
        m_BeginSimECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        // Find the ECB system once and store it for later usage



    }

    protected override void OnUpdate()
    {
        if (bulletPrefab == Entity.Null)
        {
            bulletPrefab = GetSingleton<BulletAuthoringComponent>().Prefab;
            return;

        }
        //if the BulletPrefab prefabs is empty we populate it with the bulletPrefab prefab from the prefabs collection

        var commandBuffer = m_BeginSimECB.CreateCommandBuffer().AsParallelWriter();
        // we create a new command buffer from the commmand buffer hanlde as a parralel writer

        var BulletPrefab = bulletPrefab;
        //storing the bulletPrefab in a new variable for use inside the lambda function
        Entities.
           WithAny<EnemyTag>().
         ForEach((Entity entity, int nativeThreadIndex, ref Movable shootData, ref Translation translation, in Rotation rotation) => {
             if (shootData.shoot)
             {
                //if the enemy controller has flagged the enemy to shoot

                 var bulletEntity = commandBuffer.Instantiate(nativeThreadIndex, BulletPrefab);
                 //Recording spwaning bullet for latyer playback
                 var newPosition = translation;
                 //givving the bulet the enemies possition as origin
                 var newRot = rotation;
                 //and the enemy rotation, wich will be facing the player
                 var newTag = new EnemyBulletTag {  };
                 //taggin s an enemy bullet
                 commandBuffer.SetComponent(nativeThreadIndex, bulletEntity, newPosition);
                 commandBuffer.SetComponent(nativeThreadIndex, bulletEntity, newRot);
                 
                 commandBuffer.AddComponent(nativeThreadIndex, bulletEntity, newTag);
                 //and adding the created components to the new bullet entity for later playback 
                 
                 shootData.shoot = false;
                 //we had shot so stop shooting
             }


         }).ScheduleParallel();

        //This will add our dependency to be played back on the BeginSimulationEntityCommandBufferSystem

        m_BeginSimECB.AddJobHandleForProducer(this.Dependency);
    }
}
