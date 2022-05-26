using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
[InternalBufferCapacity(7)]

[GenerateAuthoringComponent]
public struct PowerUpDataBuffer : IBufferElementData
{
    public PowerUpData PowerUpDataSet;
}
[Serializable]
public struct PowerUpData
{
    public char type;
    public float time;
    public float currentTime;

}