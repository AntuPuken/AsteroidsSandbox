
using UnityEngine;
using Unity.Entities;
[GenerateAuthoringComponent]
public struct InputData : IComponentData
{
  
    public KeyCode rightKey;
    public KeyCode leftKey;
    public KeyCode thrustKey;
    public KeyCode teleportKey;
    public KeyCode shootKey;
}
