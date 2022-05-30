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
    //creating the Command buffer system variable

    private Entity shieldPrefab;
    private Entity fasterBulletsPrefab;
    //creating the shield and faster bullets entity variables
    private EntityQuery shieldQuery;
    private EntityQuery fasterBulletsQuery;
    //this querys will be used to get the current amount of spawned shields and faster bullets 

    private int shieldAmmount = 1;
    private int fasterBulletsAmmount =1;

    private float spawnShieldInterval =10f;
    private float spawnFasterBulletsInterval =20f;
    //creating and setting the respawn time for the shields and faster bullets 
    private float currentFasterBulletsTime;
    private float currentShieldTime;
    //creating variables for keeping time since last spawned shield and faster bullets
    private bool shouldSpawnShield = false;
    private bool shouldSpawnFasterBullets = false;
    //creating auxiliary variables to know if the job should spawn or not shield and faster bullets
    protected override void OnCreate()
    {
        shieldQuery = GetEntityQuery(ComponentType.ReadWrite<ShieldTag>());
        //when we create the system we get the query for all entities with a shield tag 

        fasterBulletsQuery = GetEntityQuery(ComponentType.ReadWrite<FasterBulletsTag>());
        //when we create the system we get the query for all entities with a faster bullets tag 

        m_BeginSimECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        // Find the ECB system once and store it for later usage

    }
    protected override void OnUpdate()
    {
        if (shieldPrefab == Entity.Null || fasterBulletsPrefab == Entity.Null)
        {
            shieldPrefab = GetSingleton<PickableShieldAuthoringComponent>().Prefab;
            fasterBulletsPrefab = GetSingleton<PickableFasterBulletsAuthoringComponent>().Prefab;

            return;
        }
        //if the shield prefab  or fasterBullets prefab are empty we populate it with the corresponding prefabs from the prefabs collection

        var dt = Time.DeltaTime;
        //getting elapsed time since last frame

        var commandBuffer = m_BeginSimECB.CreateCommandBuffer();
        // we create a new command buffer from the commmand buffer hanlde

        var rand = new Unity.Mathematics.Random((uint)Stopwatch.GetTimestamp());
        //storing a handle for using random library, initialized with current time so we get a different random initializaton each frame

        var shieldCount = shieldQuery.CalculateEntityCountWithoutFiltering();
        //getting amount of spwaned shieldCount 

        var fasterBulletsCount = fasterBulletsQuery.CalculateEntityCountWithoutFiltering();
        //getting amount of spwaned fasterBullets 

        var ShieldPrefab = shieldPrefab;
        var FasterBulletsPrefab = fasterBulletsPrefab;
        //storing the shieldPrefab and fasterBulletsPrefab in new variables for use inside the lambda function

        currentFasterBulletsTime += dt;
        currentShieldTime += dt;
        //incrementing the current shild an fasterbullets current time acorrding to the elapsed time since las frame
        if (currentFasterBulletsTime > spawnFasterBulletsInterval)
        {
        //if the current time has passed the dessired time  
            shouldSpawnShield = true;
            //the job should spawn a shield
            currentFasterBulletsTime = 0;
            //and reset the current timer 
        }
        if (currentShieldTime > spawnShieldInterval)
        {
        //if the current time has passed the dessired time  

            shouldSpawnFasterBullets = true;
            //the job should spawn a fasterBullet 

            currentShieldTime = 0;
            //and reset the current timer 

        }

        var ShouldSpawnShield = shouldSpawnShield;
        var ShouldSpawnFasterBullets = shouldSpawnFasterBullets;
        //storing the shouldSpawnShield and shouldSpawnFasterBullets in new variables for use inside the lambda function
         var  ShieldAmmount = shieldAmmount;
        var FasterBulletsAmmount = fasterBulletsAmmount;
        //storing the shieldAmmount and fasterBulletsAmmount in new variables for use inside the lambda function


        Job
        .WithCode(() => {
        //declaring the job
            if (ShouldSpawnShield)
            {
            //if the job should spawn a shield
                for (int i = shieldCount; i < ShieldAmmount; ++i)
                {
                    //shieldCount will be the amount of spawned shields and the ShieldAmmount is the amount we want to spwan

                    var newEntity = commandBuffer.Instantiate(ShieldPrefab);
                    //on our command buffer we record creating an entity from our Shield prefab


                    var pos = new Translation { Value = new float3(rand.NextFloat(-8, 8), rand.NextFloat(-4, 4), 0f) };
                    //creating a new random position for the shield restricted  roughly by the screen borders

                   
                    commandBuffer.SetComponent(newEntity, pos);
                    //we then set the Translation component of the newShield equal to our new translation component

                }

            }
            if (ShouldSpawnFasterBullets) {
            //if the job should spawn a fasterBullets

                for (int i = fasterBulletsCount; i < FasterBulletsAmmount; ++i)
                {
                //fasterBulletsCount will be the amount of spawned faster bullets and the FasterBulletsAmmount is the amount we want to spwan

                    var newEntity = commandBuffer.Instantiate(FasterBulletsPrefab);
                    //on our command buffer we record creating an entity from our FasterBullets prefab



                    var pos = new Translation { Value = new float3(rand.NextFloat(-8, 8), rand.NextFloat(-4, 4), 0f) };
                    //creating a new random position for the new fasterBullets entity restricted roughly by the screen borders

                 
                    commandBuffer.SetComponent(newEntity, pos);
                    //we then set the Translation component of the new fasterBullets equal to our new translation component

                }
            }
        



   }).Schedule();
        //finally we schedule the job  

        m_BeginSimECB.AddJobHandleForProducer(Dependency);
        //add our dependency to be played back on the BeginSimulationEntityCommandBuffer

        if (shouldSpawnShield){ shouldSpawnShield = false;}
        if(shouldSpawnFasterBullets){ shouldSpawnFasterBullets = false; }
        //and if the job has done his thing, reset the flags for spawning
    }
}
