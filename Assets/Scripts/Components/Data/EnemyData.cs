using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct EnemyData : IComponentData

{
    public float shootRechargeTime ;
    public float changeDirectionRechargeTime;
    public float shootCurrentTime;
    public float changeCurrentTime;
    public float3 lastDirection;
}
