using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class BulletMoveSystem : SystemBase
{
   
    protected override void OnUpdate()
    {
        float dt = Time.DeltaTime;
        //getting the time since last frame     
   
        Entities.
        WithAny<BulletTag>().
        ForEach((ref Translation translation, in Rotation rotation, in Movable moveData) =>
        {
           //modifing the direction according to the rotation value and the upward vector  
               var direction = math.normalize(math.mul(rotation.Value, math.up())); 

            translation.Value += direction * moveData.speed * dt;
            //and adding the direction to the current position acoording to the desired speed and the time since last frame
         }).ScheduleParallel();
     
    }
}
