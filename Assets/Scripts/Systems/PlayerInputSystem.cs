using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using System;
using UnityEngine;

public class PlayerInputSystem : SystemBase
{
    float teleportRechargeTime = 5f;
    float currentTime = 5f;
    //creating variables for keeping track of the teleport timer
 
    protected override void OnUpdate()
    {
        float dt = Time.DeltaTime;
        currentTime += dt;
        //incremeting current time according to elasped time since last frame
    
        Entities.
            WithAny<PlayerTag>().
            WithoutBurst().
            ForEach(( ref Movable mov,  ref ShootData shootData, in Translation translation, in InputData inputData) => {
            bool isRightKeyPressed = Input.GetKey(inputData.rightKey);
            bool isLeftKeyPressed = Input.GetKey(inputData.leftKey);
            bool isThrutKeyPressed = Input.GetKey(inputData.thrustKey);
            bool isShootKeyPressed = Input.GetKey(inputData.shootKey);
            bool isTeleportKeyPressed = Input.GetKey(inputData.teleportKey);
               //getting pressed keys
            if (isRightKeyPressed)
            {
               mov.turnDirection = 1;
            }else if (isLeftKeyPressed)
                  {
                    mov.turnDirection = -1;
                  }
                  else
                  {
                    mov.turnDirection = 0;
                  }
                //assigning turn direction for the PlayerMoveSystem to move the player


                if ((currentTime > teleportRechargeTime) && isTeleportKeyPressed)
            {
            //if the current time has passed the dessired time  

                mov.teleport = true;
                //flag teleport for the PlayerTeleportSystem to teleport the player

                currentTime = 0;
                //and reset the current timer 

            }


            mov.forward = Convert.ToInt32(isThrutKeyPressed);
            // if isThrutKeyPressed flag mov.forward for the PlayerMoveSystem to move the player

            if (isShootKeyPressed)
            {
            // if the shoot key is pressed

                shootData.playerPos = translation.Value;
                // set playerpos with the current player position   

                mov.shoot = true;
                //and flag mov.shoot for the PlayerShootSystem to shoot

            }
            else
            {
                mov.shoot = false;
                //dont shoot
            }

        }).Run();
        //because we are reading player inuts this code has to run on the main thread so we use .Run()
    }
}
