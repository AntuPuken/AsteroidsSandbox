using System.Diagnostics;
using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using UnityEngine;

public class AsteroidSpawnSystem : SystemBase
{
    private float asteroidsAmount = 5;
    //The amount of big asteroid spawned

    private EntityQuery asteroidQuery;
    //this query will be used to get the current amount of spawned asteroids 
    
    private BeginSimulationEntityCommandBufferSystem m_BeginSimECB;
    //creating the Command buffer system variable

    private Entity asteroidPrefab;
    //creating the asteroid prefab variable
    protected override void OnCreate()
    {
        asteroidQuery = GetEntityQuery(ComponentType.ReadWrite<AsteroidTag>());
        //when we create the system we get the query for all entities with an asteroid tag

        m_BeginSimECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        // Find the ECB system once and store it for later usage
    }

    protected override void OnUpdate()
    {
     
        if (asteroidPrefab == Entity.Null)
        {
            asteroidPrefab = GetSingleton<AsteroidAuthoringComponent>().Prefab;

               return;
        }
        //if the asteroid prefab is empty we populate it with the asteroid prefab from the prefabs collection

           var commandBuffer = m_BeginSimECB.CreateCommandBuffer();
         var count = asteroidQuery.CalculateEntityCountWithoutFiltering();
        // get the current amount of asteoids spawned
      
        var AsteroidPrefab = asteroidPrefab;
        var amount = asteroidsAmount;
        var cam = Camera.main;
        var screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        float2 worldMax = new float2(screenBounds.x+10, screenBounds.y+10);
        var rand = new Unity.Mathematics.Random((uint)Stopwatch.GetTimestamp());
        // and create the new variables for use inside the job 


        Job
        .WithCode(() => {
            
            for (int i = count; i < amount; ++i)
            {
           //this for will execute until the amount of asteroids spawned is the same as the amount of asteroids we want to spawn    
                Movable moveData = new Movable()
                {
                    speed = 2,
                      forward = 0,
                    turnDirection = 0,
                    turningSpeed = 0,
                };
                //creating a new movable data, and assigning the speed of the asteroid, turn variables will be populated by the Asteroid move system
                var pos = new Translation { Value = new float3(rand.NextFloat(-1f * ((worldMax.x) / 2), ((worldMax.x) / 2)), rand.NextFloat( ( (worldMax.y) / 2), (-1f * (worldMax.y) / 2)) ,0f) };
                //creating a new random position for the asteroid restricted by the screen borders


                //on our command buffer we record creating an entity from our Asteroid prefab
                var newEntity = commandBuffer.Instantiate(AsteroidPrefab);

                //we then set the Translation component of the Asteroid prefab equal to our new translation component
                commandBuffer.SetComponent(newEntity, pos);
            }
        }).Schedule();

        //This will add our dependency to be played back on the BeginSimulationEntityCommandBuffer
        m_BeginSimECB.AddJobHandleForProducer(Dependency);
    }
}


