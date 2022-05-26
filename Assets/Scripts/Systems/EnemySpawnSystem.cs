
using System.Diagnostics;
using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using UnityEngine;

public class EnemySpawnSystem : SystemBase
{
  //  private float asteroidsAmount = 5;

    private EntityQuery enemyQuery;
    private BeginSimulationEntityCommandBufferSystem m_BeginSimECB;
    private Entity enemyPrefab;
    private float respawnTime = 10;
    protected override void OnCreate()
    {
        enemyQuery = GetEntityQuery(ComponentType.ReadWrite<EnemyTag>());
        m_BeginSimECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
    }
   
    protected override void OnUpdate()
    {

      
        respawnTime -= Time.DeltaTime;

       if (respawnTime < 0)
        {
            if (enemyPrefab == Entity.Null)
            {
                enemyPrefab = GetSingleton<EnemyAuthoringComponent>().Prefab;

                return;
            }

            float dt = Time.DeltaTime;

            var commandBuffer = m_BeginSimECB.CreateCommandBuffer();

            var count = enemyQuery.CalculateEntityCountWithoutFiltering();

            var EnemyPrefab = enemyPrefab;
            var amount = 1;


            var rand = new Unity.Mathematics.Random((uint)Stopwatch.GetTimestamp());


            Job
        .WithCode(() => {
            for (int i = count; i < amount; ++i)
            {
               
                var pos = new Translation { Value = new float3(rand.NextFloat(-8, 8), rand.NextFloat(-4, 4), 0f) };

                    var e = commandBuffer.Instantiate(EnemyPrefab);
                 commandBuffer.SetComponent(e, pos);
                      }
        }).Schedule();
            respawnTime = 10;
      
        m_BeginSimECB.AddJobHandleForProducer(Dependency);
        }
    }
}