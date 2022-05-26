
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct PlayerData : IComponentData
{
    public float playerLifes;
    public float currentLifes;
    public float currentTime;
    public float shootRechargeTime;
}
