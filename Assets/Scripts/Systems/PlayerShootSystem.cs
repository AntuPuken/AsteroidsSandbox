using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class PlayerShootSystem : SystemBase
{
    private Entity m_BulletPrefab;
    private BeginSimulationEntityCommandBufferSystem m_BeginSimECB;
       protected override void OnCreate()
    {
        m_BeginSimECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();



    }
  
    protected override void OnUpdate()
    {
        if (m_BulletPrefab == Entity.Null)
        {
            m_BulletPrefab = GetSingleton<BulletAuthoringComponent>().Prefab;
            return;

        }  
        var commandBuffer = m_BeginSimECB.CreateCommandBuffer().AsParallelWriter();
        var bulletPrefab = m_BulletPrefab;
        Entities.
           WithAny<PlayerTag>().
         ForEach((Entity entity, int nativeThreadIndex,ref Movable moveData, ref Translation translation, ref ShootData shootData,in Rotation rotation ) => {
               if (moveData.shoot && shootData.shoot)
               {
                    var bulletEntity = commandBuffer.Instantiate(nativeThreadIndex, bulletPrefab);
                    var newPosition = translation;
                    var newRot = rotation;
                    var newv = new PlayerBulletTag { };
                    commandBuffer.AddComponent(nativeThreadIndex, bulletEntity, newv);
                    commandBuffer.SetComponent(nativeThreadIndex, bulletEntity, newPosition);
                    commandBuffer.SetComponent(nativeThreadIndex, bulletEntity, newRot);

                    shootData.shoot = false;
                    moveData.shoot = false;
            
}
                
            
         }).ScheduleParallel();

     
        m_BeginSimECB.AddJobHandleForProducer(Dependency);
    }
}
