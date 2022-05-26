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
        Entities.
            WithAny<PlayerTag>().
            ForEach((ref Movable mov, ref PhysicsVelocity vel, ref Rotation rot) => {
               quaternion normalizedRot = math.normalize(rot.Value);
               quaternion angleToRotate = quaternion.AxisAngle(math.back(), mov.turningSpeed * mov.turnDirection * dt);
               rot.Value = math.mul(normalizedRot, angleToRotate);
            var direction = math.mul(rot.Value, new float3(0f, 1f, 0f));
            vel.Linear.xyz = math.clamp( vel.Linear.xyz + direction * mov.forward * mov.speed * dt,-mov.speed, mov.speed);
         }).ScheduleParallel();
    }
}
