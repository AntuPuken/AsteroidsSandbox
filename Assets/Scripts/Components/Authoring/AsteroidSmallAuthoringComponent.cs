using Unity.Entities;

[GenerateAuthoringComponent]
public struct AsteroidSmallAuthoringComponent : IComponentData
{
    public Entity Prefab;
}