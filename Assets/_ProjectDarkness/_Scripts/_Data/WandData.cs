using UnityEngine;

namespace ProjectDarkness
{
    [CreateAssetMenu(fileName = "New Wand Data", menuName = "ProjectDarkness/WandData")]
    public class WandData : ScriptableObject
    {
        [field: SerializeField] public string WandName { get; private set; }
        [field: SerializeField] public int ManaAmount { get; private set; } = 80;
        [field: SerializeField] public float ManaRegenPerSec { get; private set; } = 10;
        [field: SerializeField] public float CastDelayTime { get; private set; } = 0.15f;
        [field: SerializeField] public float CooldownTime { get; private set; } = 0.5f;
        [field: SerializeField] public float Scatter { get; private set; } = 0;
        [field: SerializeField] public int Capacity { get; private set; } = 3;
        
    }
}
