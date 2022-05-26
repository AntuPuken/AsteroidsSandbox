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
        var BulletPrefab = bulletPrefab;
        Entities.
           WithAny<EnemyTag>().
         ForEach((Entity entity, int nativeThreadIndex, ref Movable shootData, ref Translation translation, in Rotation rotation) => {
             if (shootData.shoot)
             {
                 var bulletEntity = commandBuffer.Instantiate(nativeThreadIndex, BulletPrefab);
                 var newPosition = translation;
                 var newRot = rotation;
                 var newv = new EnemyBulletTag {  };
                 commandBuffer.SetComponent(nativeThreadIndex, bulletEntity, newPosition);
                 commandBuffer.SetComponent(nativeThreadIndex, bulletEntity, newRot);
                 //
                 commandBuffer.AddComponent(nativeThreadIndex, bulletEntity, newv);//add component cos SetComponent resulted in memory leak
                 //
                 shootData.shoot = false;

             }


         }).ScheduleParallel();


        m_BeginSimECB.AddJobHandleForProducer(this.Dependency);
    }
}
