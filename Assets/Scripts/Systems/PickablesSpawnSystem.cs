using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using System.Diagnostics;
public class PickablesSpawnSystem : SystemBase
{
    private BeginSimulationEntityCommandBufferSystem m_BeginSimECB;
    private Entity shieldPrefab;
    private Entity fasterBulletsPrefab;
    private EntityQuery shieldQuery;
    private EntityQuery fasterBulletsQuery;
    private float spawnShieldInterval =10f;
    private float spawnFasterBulletsInterval =20f;
    private float currentFasterBulletsTime;
    private float currentShieldTime;
    private bool shouldSpawnShield = false;
    private bool shouldSpawnFasterBullets = false;
    protected override void OnCreate()
    {
        shieldQuery = GetEntityQuery(ComponentType.ReadWrite<ShieldTag>());
        fasterBulletsQuery = GetEntityQuery(ComponentType.ReadWrite<FasterBulletsTag>());
        m_BeginSimECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        if (shieldPrefab == Entity.Null || fasterBulletsPrefab == Entity.Null)
        {
            shieldPrefab = GetSingleton<PickableShieldAuthoringComponent>().Prefab;
            fasterBulletsPrefab = GetSingleton<PickableFasterBulletsAuthoringComponent>().Prefab;

            return;
        }

        var dt = Time.DeltaTime;
        var commandBuffer = m_BeginSimECB.CreateCommandBuffer();
        var rand = new Unity.Mathematics.Random((uint)Stopwatch.GetTimestamp());
        var shieldCount = shieldQuery.CalculateEntityCountWithoutFiltering();
        var fasterBulletsCount = fasterBulletsQuery.CalculateEntityCountWithoutFiltering();
        var ShieldPrefab = shieldPrefab;
        var FasterBulletsPrefab = fasterBulletsPrefab;
        currentFasterBulletsTime += dt;
        currentShieldTime += dt;
        if (currentFasterBulletsTime > spawnFasterBulletsInterval)
        {
            shouldSpawnShield = true;
            currentFasterBulletsTime = 0;
        }
        if (currentShieldTime > spawnShieldInterval)
        {
            shouldSpawnFasterBullets = true;
            currentShieldTime = 0;
        }

        var ShouldSpawnShield = shouldSpawnShield;
        var ShouldSpawnFasterBullets = shouldSpawnFasterBullets;


        Job
        .WithCode(() => {
            if (ShouldSpawnShield)
            {
                for (int i = shieldCount; i < 1f; ++i)
                {
                    // this is how much within perimeter asteroids start


                    var pos = new Translation { Value = new float3(rand.NextFloat(-8, 8), rand.NextFloat(-4, 4), 0f) };

                    //on our command buffer we record creating an entity from our Asteroid prefab
                    var newEntity = commandBuffer.Instantiate(ShieldPrefab);

                    //we then set the Translation component of the Asteroid prefab equal to our new translation component
                    commandBuffer.SetComponent(newEntity, pos);
                }

            }
            if (ShouldSpawnFasterBullets) {
                for (int i = fasterBulletsCount; i < 1f; ++i)
                {


                    var pos = new Translation { Value = new float3(rand.NextFloat(-8, 8), rand.NextFloat(-4, 4), 0f) };

                    var newEntity = commandBuffer.Instantiate(FasterBulletsPrefab);

                    commandBuffer.SetComponent(newEntity, pos);
                }
            }
        



   }).Schedule();
    m_BeginSimECB.AddJobHandleForProducer(Dependency);

        if(shouldSpawnShield){ shouldSpawnShield = false;}
        if(shouldSpawnFasterBullets){ shouldSpawnFasterBullets = false; }
    }
}
