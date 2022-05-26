using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using UnityEngine;
using System.Diagnostics;

//[UpdateBefore(typeof(FixedStepSimulationSystemGroup))]
public class PlayerSpawnerSystem : SystemBase
{

    private EntityQuery playerQuery;
    private BeginSimulationEntityCommandBufferSystem m_BeginSimECB;

    private SpawnPlayerData spawnPlayerData;
    private Entity playerPrefab;
    private EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

   

    public BufferFromEntity<PowerUpDataBuffer> lookup;

    protected override void OnStartRunning()
    {
        playerQuery = GetEntityQuery(ComponentType.ReadWrite<PlayerTag>());
        m_BeginSimECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        

    }
    protected override void OnUpdate()
    {
        if (playerPrefab == Entity.Null)
        {
            playerPrefab = GetSingleton<PlayerAuthoringComponent>().Prefab;
            return;
        }
        var commandBuffer = m_BeginSimECB.CreateCommandBuffer();
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

        var playerCount = playerQuery.CalculateEntityCountWithoutFiltering();

        if (playerCount == 0)
        {
            // var playerData = playerQuery.ToComponentDataArray<SpawnPlayerData>(Allocator.Temp);

            commandBuffer.Instantiate(playerPrefab);
        }
        var PlayerPrefab = playerPrefab;
        var rand = new Unity.Mathematics.Random((uint)Stopwatch.GetTimestamp());


        Entities.
               WithAny<DestroyPlayerTag>().
               ForEach((Entity entity, int entityInQueryIndex, ref PlayerData playerData, ref Translation translation) => {
                   if (playerData.currentLifes > 0)
                   {
                       var newEntity = commandBuffer.Instantiate(PlayerPrefab);

                       var newPlayerData = new PlayerData { playerLifes = playerData.playerLifes, currentLifes = playerData.currentLifes - 1, currentTime = 0, shootRechargeTime = 1 };
                       var pos = new Translation { Value = new float3(rand.NextFloat(-8, 8), rand.NextFloat(-4, 4), 0f) };
                       
                       commandBuffer.AddComponent(newEntity, newPlayerData);
                       commandBuffer.AddComponent(newEntity, pos);
                       commandBuffer.AddComponent(newEntity,new RefreshUIPlayerLifesTag { });
                       ecb.DestroyEntity(entity);
                   }
                   else
                   {
                       var newEntity = commandBuffer.CreateEntity();
                       commandBuffer.AddComponent(newEntity, new ResetUIScoreTag { });
                       ecb.DestroyEntity(entity);
                       //finish the game
                   }

               }).Schedule();
    
      

        m_BeginSimECB.AddJobHandleForProducer(this.Dependency);
        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);

    }

}
