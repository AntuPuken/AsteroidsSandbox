using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class DestroyAsteroidSystem : SystemBase
{
    private Entity m_MediumAsteroidPrefab;
    private Entity m_SmallAsteroidPrefab;
    //here we need the medium and small asteroid prefabs to spawn when an asteroid is destroyed 
    private BeginSimulationEntityCommandBufferSystem m_BeginSimECB;

    private EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    //and well need command buffers on the star and end of every frame 



    protected override void OnStartRunning()
    {
        base.OnCreate();

        m_BeginSimECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

        // Find the ECB system once and store it for later usage
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();


        }
    protected override void OnUpdate()

    {
        if (m_MediumAsteroidPrefab == Entity.Null || m_SmallAsteroidPrefab == Entity.Null)
        {
            m_MediumAsteroidPrefab = GetSingleton<AsteroidMediumAuthoringComponent>().Prefab;
            m_SmallAsteroidPrefab = GetSingleton<AsteroidSmallAuthoringComponent>().Prefab;
            return;

        }
        //if the asteroids prefabs are empty we populate them with the asteroid prefabs from the prefabs collection


        var mediumAsteroidPrefab = m_MediumAsteroidPrefab;
        var smallAsteroidPrefab = m_SmallAsteroidPrefab;
        //creating variables for use inside the lambda expression
        var commandBuffer = m_BeginSimECB.CreateCommandBuffer().AsParallelWriter();
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();
        // and new command buffers as paralel wirters for the beggin and end of every frame 
        
      

        Entities.
        WithAny<DestroyAsteroidTag>().
        ForEach((Entity entity, int entityInQueryIndex,  ref Translation translation, ref AsteroidData asteroidData, in Rotation rotation) => {
           //the asteroid type determines if is a small(0), medium(1) or big Ateroid(2)
            if (asteroidData.asteroidType == 0f)
            {
             //if is the smallets asteroid we just destroy it at the end of frame
                ecb.DestroyEntity(entityInQueryIndex, entity);
                
            }
            else

            if (asteroidData.asteroidType == 1f)
            {
                //if is a medium asteroid we spawn two new small asteroids
                var asteroidEntity = commandBuffer.Instantiate(entityInQueryIndex, smallAsteroidPrefab);
                var asteroidEntity2 = commandBuffer.Instantiate(entityInQueryIndex, smallAsteroidPrefab);
           
                
                var newPosition = translation;
                var newRot = rotation;
                //we assign the rotation and location of the original ateroid
                
                var newAsteroidData = new AsteroidData { asteroidType=0f };
                //and we assing the type  to the small type
                
                commandBuffer.SetComponent(entityInQueryIndex, asteroidEntity, newAsteroidData);
                commandBuffer.SetComponent(entityInQueryIndex, asteroidEntity, newPosition);  
                commandBuffer.SetComponent(entityInQueryIndex, asteroidEntity2, newAsteroidData);
                commandBuffer.SetComponent(entityInQueryIndex, asteroidEntity2, newPosition);
                //we add the rotation and location components to the new spawned asteroids
                ecb.DestroyEntity(entityInQueryIndex, entity);
                //and destroy the original asteroid at the end of the frame

            }
            else 
                if (asteroidData.asteroidType == 2f)
                {
                //if is a big asteroid we spawn two new medium asteroids
                var asteroidEntity = commandBuffer.Instantiate(entityInQueryIndex, mediumAsteroidPrefab);
                var asteroidEntity2 = commandBuffer.Instantiate(entityInQueryIndex, mediumAsteroidPrefab);
              
                
                var newPosition = translation;
                var newRot = rotation;
                //we assign the rotation and location of the original ateroid

                var newAsteroidData = new AsteroidData { asteroidType = 1f };
                //and we assing the type  to the medium type

                commandBuffer.SetComponent(entityInQueryIndex, asteroidEntity, newAsteroidData);
                commandBuffer.SetComponent(entityInQueryIndex, asteroidEntity, newPosition);
                commandBuffer.SetComponent(entityInQueryIndex, asteroidEntity2, newAsteroidData);
                commandBuffer.SetComponent(entityInQueryIndex, asteroidEntity2, newPosition);
                //we add the rotation and location components to the new spawned asteroids



                ecb.DestroyEntity(entityInQueryIndex, entity);
                //and destroy the original asteroid at the end of the frame

               // asteroidData.asteroidType = 1f;


            }


        }).ScheduleParallel();

        //This will add our dependency to be played back on the BeginSimulationEntityCommandBuffer
        m_BeginSimECB.AddJobHandleForProducer(this.Dependency);
        //This will add our dependency to be played back on the EndSimulationEntityCommandBufferSystem
        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);

    }
}

