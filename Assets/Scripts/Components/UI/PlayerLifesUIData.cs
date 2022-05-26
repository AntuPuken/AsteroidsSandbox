using Unity.Entities;
using UnityEngine.UI;

    [GenerateAuthoringComponent]
    public class PlayerLifesUIData : IComponentData
    {
        public Text lifesText;
    }
