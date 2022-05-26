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
   protected override void OnCreate()
    {
      
    }
    protected override void OnUpdate()
    {
        float dt = Time.DeltaTime;
        currentTime += dt;

    
        Entities.
            WithAny<PlayerTag>().
            WithoutBurst().
            ForEach(( ref Movable mov,  ref ShootData shootData, in Translation translation, in InputData inputData) => {
            bool isRightKeyPressed = Input.GetKey(inputData.rightKey);
            bool isLeftKeyPressed = Input.GetKey(inputData.leftKey);
            bool isThrutKeyPressed = Input.GetKey(inputData.thrustKey);
            bool isShootKeyPressed = Input.GetKey(inputData.shootKey);
            bool isTeleportKeyPressed = Input.GetKey(inputData.teleportKey);
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
            
             if((currentTime > teleportRechargeTime) && isTeleportKeyPressed)
                {
                    mov.teleport = true;
                    currentTime = 0;
                }   

    
            
            mov.forward = Convert.ToInt32(isThrutKeyPressed);
               
                if (isShootKeyPressed)
                {
                  
                    shootData.playerPos = translation.Value;
                    mov.shoot = true;
             
                }
                else
                {
                    mov.shoot = false;
                }

        }).Run();
     
    }
}
