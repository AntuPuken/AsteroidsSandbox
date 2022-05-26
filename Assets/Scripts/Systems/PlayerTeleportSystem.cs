using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using UnityEngine;
using System.Diagnostics;

public class PlayerTeleportSystem : SystemBase
{
    protected override void OnUpdate()
    {


        var random = new Unity.Mathematics.Random((uint)Stopwatch.GetTimestamp());



        Entities.
        WithAny<PlayerTag>().
            ForEach((ref Translation translation, ref PhysicsVelocity vel, ref Movable mov) => {
                if (mov.teleport == true)
                {
                    mov.teleport = false;
                    translation.Value.y = random.NextFloat(-5f,5f);
                    translation.Value.x = random.NextFloat(-5f, 5f);
                    vel.Linear.xyz -= vel.Linear.xyz;
                }
        }).ScheduleParallel();
    }
}
