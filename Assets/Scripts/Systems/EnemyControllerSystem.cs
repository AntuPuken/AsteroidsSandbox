using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;


public class EnemyControllerSystem : SystemBase
{
    private Entity playerEntity;
    //here we need player entiy for the enemy's target
  
   
    protected override void OnUpdate()
    {
        RequireSingletonForUpdate<PlayerTag>();
        //only update when there's a player present

        playerEntity = GetSingletonEntity<PlayerTag>();
     // get a reference to the current spawned player
        float dt = Time.DeltaTime;
      //get elapsed time since last frame
        
        Entities.
        WithAny<EnemyTag>().
       ForEach(( ref TargetData targetData) => {
           targetData.targetEntity = playerEntity;
       }).WithoutBurst().Run();
        //Assing the enemy target entity to the current spawned player
        
        Entities.
         WithAny<EnemyTag>().
         ForEach((ref EnemyData enemyData, ref Movable shootData, ref Rotation rotation, ref TargetData targetData, in Translation pos) => {
             ComponentDataFromEntity<Translation> allTranslations = GetComponentDataFromEntity<Translation>(true);
              
             //get all spawned translations 
             
             if (!allTranslations.HasComponent(targetData.targetEntity))
             {
                 return;
             }
             //if there's no translation corresponding to the target entity (the player) stop executing 

             Translation targetpos = allTranslations[targetData.targetEntity];
            //but if there is, get the player translation 
             float3 playerPos = targetpos.Value - pos.Value;
            //and offset it by the enemy position to get the distance vector to the player

             FaceDirection(ref rotation, playerPos);
             //finally face the enemy to the player position
         }).ScheduleParallel();

        

            Entities.
            WithAny<EnemyTag>().
            ForEach((ref EnemyData enemyData, ref Movable shootData,ref Rotation rotation, in Translation pos, in TargetData targetData) => {
        //add the elapsed time to the current time
                enemyData.shootCurrentTime += dt;

                if (enemyData.shootCurrentTime > enemyData.shootRechargeTime)
                {              
                    shootData.shoot = true;              
                    enemyData.shootCurrentTime = 0;
                }
         //and if the elapsed time is greater than the dessired time reset the current time and assign shoot as true for the EnemyShootSystem to shoot


            }).ScheduleParallel();



        Entities.
          WithAny<EnemyTag>().
          ForEach((ref EnemyData enemyData, ref Movable shootData, ref Rotation rotation, ref Translation pos, in TargetData targetData) => {
              enemyData.changeCurrentTime += dt;
              //add the elapsed time to the current time
            
              if (enemyData.changeCurrentTime > enemyData.changeDirectionRechargeTime)
              {
                  //if the elapsed time is greater than the dessired time reset the current time refresh the direction in which the enemy moves  

                  var direction = math.normalize(math.mul(rotation.Value, math.up()));

                  //acording to its rotation and upwards vector

                  enemyData.changeCurrentTime = 0;
                  enemyData.lastDirection = direction; 
                }

                pos.Value += enemyData.lastDirection * dt;
                //but always move the enemy in the last known direction   

          }).ScheduleParallel();



    }

    //Auxiliary function for facing direction according to current rotation and a direction vector

    private static void FaceDirection(ref Rotation rot, float3 moveData)
    {
        float angle = math.atan2(moveData.y, moveData.x);
        quaternion tg = quaternion.AxisAngle(new float3(0f, 0f, 1f), angle - 89.54f);
        rot.Value = math.normalize(tg);

    }
}

