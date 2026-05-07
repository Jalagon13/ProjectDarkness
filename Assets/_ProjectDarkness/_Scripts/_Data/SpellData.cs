using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectDarkness
{
    [CreateAssetMenu(fileName = "New Spell Data", menuName = "ProjectDarkness/SpellData")]
    public class SpellData : ScriptableObject
    {
        [field: SerializeField] public string SpellName { get; private set; }
        [field: SerializeField, Required] public Spell SpellPrefab { get; private set; }
        [field: SerializeField] public Sprite UiDisplay { get; private set; }

        [field: PropertySpace(5)]
        [field: Header("Spell Base Stats")]
        [field: SerializeField] public int ManaDrain { get; private set; } = 10;
        [field: SerializeField] public float Speed { get; private set; } = 10f;
        [field: SerializeField] public float Lifetime { get; private set; } = 10f;
        
        [field: PropertySpace(5)]
        [field: Header("Combat Stats")]
        [field: SerializeField] public int Damage { get; private set; } = 5;
        [field: SerializeField] public int PierceCount { get; private set; } = 0;
        [field: SerializeField] public int BounceCount { get; private set; } = 0;
        [field: SerializeField] public int Knockback { get; private set; } = 3;
        
    }
}
