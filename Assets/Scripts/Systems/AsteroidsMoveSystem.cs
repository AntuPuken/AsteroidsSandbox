using System.Diagnostics;
using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using UnityEngine;
public class AsteroidsMoveSystem : SystemBase


{
    protected override void OnUpdate()
    {
        
        float dt = Time.DeltaTime;
        //Getting time since last fame
        var random = new Unity.Mathematics.Random((uint)Stopwatch.GetTimestamp());
        //and random variable reference initialized with current TimeStamp
        Entities.
            WithAny<AsteroidTag>().
            ForEach((Entity e, int entityInQueryIndex, ref Movable moveData, ref Translation translation, in Rotation rotation) => {
                    
           //if the asteroid doesnt have a direction value  we assing a new one    
          if (moveData.turnDirection == 0 )
                {
                    moveData.turnDirection = random.NextFloat(-1.0f, 1.0f);

                    moveData.turningSpeed = random.NextFloat(-1.0f, 1.0f);
                }
          //we assign a direction acording to the current rotation and random values to give a random direction to each asteroid 
                var direction = math.normalize(math.mul(rotation.Value, new float3(moveData.turnDirection, moveData.turningSpeed, 0f)));
             //and finally we modify the asteroid possition according to this direction, the speed of the asteoid and the time since last frame   
                translation.Value += direction * moveData.speed * dt;
             
            }).ScheduleParallel();
    }
}
