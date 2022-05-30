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
        //storing a handle for using random library, initialized with current time so we get a different random initializaton each frame



        Entities.
        WithAny<PlayerTag>().
            ForEach((ref Translation translation, ref PhysicsVelocity vel, ref Movable mov) => {
                if (mov.teleport == true)
                {
                //if the PlayerInputSystem has flagged the player to teleport

                    translation.Value.y = random.NextFloat(-5f,5f);
                    translation.Value.x = random.NextFloat(-5f, 5f);
                    //then we generate a random position inside the world borders

                    vel.Linear.xyz -= vel.Linear.xyz;
                    //and we stop the player, so is doesnt carry its velocity when he teleports
                    mov.teleport = false;
                    //finally we unflag the teleport
                }
            }).ScheduleParallel();
        //Scheduling for paralel worker threads

    }
}
