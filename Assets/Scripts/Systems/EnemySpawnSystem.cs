
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

    private EntityQuery enemyQuery;
    //creating enemy query, this will be later used to know the amount of enemies spawned 

    private BeginSimulationEntityCommandBufferSystem m_BeginSimECB;
    //creating the Command buffer system variable

    private Entity enemyPrefab;
    //creating the enemy prefab variable

    private float respawnTime = 10;
    //creating the variable for counting the time since the system started running or the last enemy was spawned

    private int enemyAmount = 1;
    //creting the amount of enemies spawned at a time variable
    protected override void OnCreate()
    {
        enemyQuery = GetEntityQuery(ComponentType.ReadWrite<EnemyTag>());
        //initializing the query with the enemy tag 
        m_BeginSimECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        // Find the ECB system once and store it for later usage

    }

    protected override void OnUpdate()
    {

      
        respawnTime -= Time.DeltaTime;
        //decrementing respawn time acording to time elapsed since last frame
       if (respawnTime < 0)
            //if respawnTime has runned out, it is time to spawn the enemy 
        {
            if (enemyPrefab == Entity.Null)
            {
                enemyPrefab = GetSingleton<EnemyAuthoringComponent>().Prefab;

                return;
            }
            //if the enemy prefab is empty populate it with the enemy prefab from the prefab collection

            float dt = Time.DeltaTime;
            //getting elapsed time since last frame
            var commandBuffer = m_BeginSimECB.CreateCommandBuffer();
            //and a handle for the Command Buffer System
            var count = enemyQuery.CalculateEntityCountWithoutFiltering();
            //getting amount of spawned enemies 
            
            var EnemyPrefab = enemyPrefab;
            //storing the enemy prefab inside  new variable for use inside the job
            var amount = enemyAmount;
            //and the dessired amount of enemies to spawn 

            var rand = new Unity.Mathematics.Random((uint)Stopwatch.GetTimestamp());
            //storing a handle for using random library, initialized with current time so we get a different random initializaton each frame

            Job
        .WithCode(() => {
            //creating a job

            for (int i = count; i < amount; ++i)
            {
            //count will be the amount of spawned enemies and the amount is the amount we want to spwan
            
                var pos = new Translation { Value = new float3(rand.NextFloat(-8, 8), rand.NextFloat(-4, 4), 0f) };
                //creating a new random position for the enemy restricted  roughly by the screen borders
                var newEntity = commandBuffer.Instantiate(EnemyPrefab);
                //on our command buffer we record creating an entity from our enemy prefab

                commandBuffer.SetComponent(newEntity, pos);
                //and we set the new position to the new created enemy for later playback on the command buffer          
            }
        }).Schedule();
            //finally we schedule the job  
            respawnTime = 10;
            //reset the timer
            m_BeginSimECB.AddJobHandleForProducer(Dependency);
            //and add our dependency to be played back on the BeginSimulationEntityCommandBuffer
        }
    }
}