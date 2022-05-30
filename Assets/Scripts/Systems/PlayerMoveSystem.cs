using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
public class MovableSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float dt = Time.DeltaTime;
        //Getting time since last fame

        Entities.
            WithAny<PlayerTag>().
            ForEach((ref Movable mov, ref PhysicsVelocity vel, ref Rotation rot) => {

                //Rotation ---------------------------------------------------------------------------------------------------------
                
                quaternion normalizedRot = math.normalize(rot.Value);
                //we normalize the rotation 
                quaternion angleToRotate = quaternion.AxisAngle(math.back(), mov.turningSpeed * mov.turnDirection * dt);
                //and create an angle to rotate, arround the unitys back axis according to the turndirection, the turningSeed and the elapsed time since last frame 
                rot.Value = math.mul(normalizedRot, angleToRotate);
                //and add the current rotation to the dessired rotation
                
                //---------------------------------------------------------------------------------------------------------


                //forward Physics Velocity  ---------------------------------------------------------------------------------------------------------

                var direction = math.mul(rot.Value, new float3(0f, 1f, 0f));
               //creating a forward float3 vector

               vel.Linear.xyz = math.clamp( vel.Linear.xyz + direction * mov.forward * mov.speed * dt,-mov.speed, mov.speed);
                //and adding this vector modulated by the dessired speed, the elapsed time since last frame, and if the mov.forward has been flagged to move the player by the PlyerInputSystem
                //Also we clamp the terminal velocity so it doesnt surpass the player's default top velocity
              
                //---------------------------------------------------------------------------------------------------------

            }).ScheduleParallel();
        //Scheduling for paralel worker threads
    }
}
