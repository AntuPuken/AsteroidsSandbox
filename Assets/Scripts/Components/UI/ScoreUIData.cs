using Unity.Entities;
using UnityEngine.UI;

    [GenerateAuthoringComponent]
public class ScoreUIData : IComponentData
{
    public Text scoreText;
}
