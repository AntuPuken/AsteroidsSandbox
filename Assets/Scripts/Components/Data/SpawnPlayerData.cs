using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct SpawnPlayerData : IComponentData
{
    public Entity playerPrefab;
    public float3 MinSpawnPosition;
    public float3 MaxSpawnPosition;
    
}
