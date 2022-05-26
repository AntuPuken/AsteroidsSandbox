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
        Entities.ForEach((ref PlayerData playerData , ref ShootData shootData) => {
            playerData.currentTime += dt;
            if (playerData.currentTime > playerData.shootRechargeTime)
            {
                shootData.shoot = true;
                playerData.currentTime = 0;
            }
        }).ScheduleParallel();
    }
}
