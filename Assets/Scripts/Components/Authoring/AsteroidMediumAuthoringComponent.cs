using Unity.Entities;

[GenerateAuthoringComponent]
public struct AsteroidMediumAuthoringComponent : IComponentData
{
    public Entity Prefab;
}