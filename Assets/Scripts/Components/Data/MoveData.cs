using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Movable : IComponentData
{
    public float speed;
    
    public float forward;
    public float turnDirection;
    public float turningSpeed;
    public bool teleport;
    public bool shoot;
}
