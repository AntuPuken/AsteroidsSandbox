using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class PlayerShootSystem : SystemBase
{
    private Entity bulletPrefab;
    //creating the bullet prefab variable

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
        //if the bullet prefab is empty we populate it with the bullet prefab from the prefabs collection

        var commandBuffer = m_BeginSimECB.CreateCommandBuffer().AsParallelWriter();
        // create new command buffer as paralel wirter for the beggin of every frame 

        var BulletPrefab = bulletPrefab;
        //creating variables for use inside the lambda expression

        Entities.
           WithAny<PlayerTag>().
         ForEach((Entity entity, int nativeThreadIndex,ref Movable moveData, ref Translation translation, ref ShootData shootData,in Rotation rotation ) => {
               if (moveData.shoot && shootData.shoot)
               {
               //if the PlayerInputSystem and the PlayerRechargeShootSystem have flagged the shoot variables as true 
               
                 
                    var bulletEntity = commandBuffer.Instantiate(nativeThreadIndex, BulletPrefab);
                    //on our command buffer we record creating an entity from our bullet prefab

                    var newPosition = translation;
                    var newRot = rotation;
                    //we get the player position and rotation  
                    var newv = new PlayerBulletTag { };
                    //and make a new playerbullet tag

                    commandBuffer.AddComponent(nativeThreadIndex, bulletEntity, newv);
                    commandBuffer.SetComponent(nativeThreadIndex, bulletEntity, newPosition);
                    commandBuffer.SetComponent(nativeThreadIndex, bulletEntity, newRot);
                    //and add or set these new components to the new created entity 
                    shootData.shoot = false;
                    moveData.shoot = false;
                 //finally we have shot so we flag the shoot variables as false so we dont shoot again until the PlayerInputSystem and the PlayerRechargeShootSystem flag them as true again 
             }


         }).ScheduleParallel();
        //Scheduling for paralel worker threads

         m_BeginSimECB.AddJobHandleForProducer(Dependency);
        //This will add our dependency to be played back on the BeginSimulationEntityCommandBuffer

    }
}
