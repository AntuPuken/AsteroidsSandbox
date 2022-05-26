using System;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct LifeExpectancyData : IComponentData
{
   public float lifeExpectancy;
    public float currentAge;
}
