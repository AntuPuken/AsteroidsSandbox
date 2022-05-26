using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct GameData : IComponentData
{
    public float playerAmount;
    public float asteroidsAmount;
    public float enemyAmount;
    public float enemyRespawnTime;
    public float score;
}
