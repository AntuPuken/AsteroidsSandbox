using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class PlayerRechargeShootSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float dt = Time.DeltaTime;
        //Getting elapsed time since last fame

        Entities.ForEach((ref PlayerData playerData , ref ShootData shootData) => {
            playerData.currentTime += dt;
            //incremeting current time according to elasped time since last frame


            if (playerData.currentTime > playerData.shootRechargeTime)
            {
            //if the current time has passed the dessired time  

                shootData.shoot = true;
                //flag shootData.shoot for the PlayerShootSystem to shoot

                playerData.currentTime = 0;
                //and reset the current timer 

            }
        }).ScheduleParallel();
        //Scheduling for paralel worker threads
    }
}
