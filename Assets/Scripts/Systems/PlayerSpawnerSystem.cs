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
    //this query will be used to get the current amount of spawned players

    private BeginSimulationEntityCommandBufferSystem m_BeginSimECB;
    private EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    //creating the Command buffer system variables 

    private Entity playerPrefab;
    //creating the player prefab variable

    protected override void OnStartRunning()
    {
        playerQuery = GetEntityQuery(ComponentType.ReadWrite<PlayerTag>());
        //initializing the query with the player tag 

        m_BeginSimECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        // Find the ECB systems once and store them for later usage



    }
    protected override void OnUpdate()
    {
        if (playerPrefab == Entity.Null)
        {
            playerPrefab = GetSingleton<PlayerAuthoringComponent>().Prefab;
            return;
        }
        //if the player prefab is empty populate it with the player prefab from the prefab collection

        var commandBuffer = m_BeginSimECB.CreateCommandBuffer();
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
        // and new command buffers as paralel wirters for the beggin and end of every frame 

        var playerCount = playerQuery.CalculateEntityCountWithoutFiltering();
        //getting amount of spawned players 

        if (playerCount == 0)
        {
        //if there aren't any players spawned
        
            commandBuffer.Instantiate(playerPrefab);
            //on our command buffer we record creating an entity from our player prefab

        }

        var PlayerPrefab = playerPrefab;
        //storing the player prefab inside  new variable for use inside the lambda expression

        var rand = new Unity.Mathematics.Random((uint)Stopwatch.GetTimestamp());
        //storing a handle for using random library, initialized with current time so we get a different random initialization each frame


        Entities.
               WithAny<DestroyPlayerTag>().
               //if there are any players scheduled for destroying

               ForEach((Entity entity, int entityInQueryIndex, ref PlayerData playerData, ref Translation translation) => {
                   
                   if (playerData.currentLifes > 0)
                   {
                    // and the player still has lifes left

                       var newEntity = commandBuffer.Instantiate(PlayerPrefab);
                       //on our command buffer we record creating an entity from our player prefab

                       var newPlayerData = new PlayerData { playerLifes = playerData.playerLifes, currentLifes = playerData.currentLifes - 1, currentTime = 0, shootRechargeTime = 1 };
                       //we create a new player data variable with the values of the player we wish to destroy but decrement the current lifes by one

                       var pos = new Translation { Value = new float3(rand.NextFloat(-8, 8), rand.NextFloat(-4, 4), 0f) };
                       //then we generate a random position inside the world borders

                       commandBuffer.AddComponent(newEntity, newPlayerData);
                       commandBuffer.AddComponent(newEntity, pos);
                       //and add these components to the newly created player entity 
                       commandBuffer.AddComponent(newEntity,new RefreshUIPlayerLifesTag { });
                       //and add a RefreshUIPlayerLifesTag for the ScoreUISystem to refresh the player lifes UI
                       }
                   else //if the player runed out of lifes
                   {
                       //finish the game
                       var newEntity = commandBuffer.CreateEntity();
                      //on our command buffer we record creating an empty entity 
                       commandBuffer.AddComponent(newEntity, new ResetUIScoreTag { });
                       //and  add a ResetUIScoreTag for the ScoreUISystem to reset the score UI back to 0
                  
                     
                   }
                   ecb.DestroyEntity(entity);
                   //finnaly we destroy the entity at the end of the simulation frame to make sure that we destroy the entity when all systems have finished ussing it

               }).Schedule();
                //sheudle the task
      

        m_BeginSimECB.AddJobHandleForProducer(this.Dependency);
        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
        //and add our dependency to be played back on the BeginSimulationEntityCommandBuffer

    }

}
