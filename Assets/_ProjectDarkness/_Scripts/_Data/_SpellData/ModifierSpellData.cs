using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectDarkness
{
    [CreateAssetMenu(fileName = "New Modifier Spell Data", menuName = "ProjectDarkness/SpellData/Modifier")]
    public class ModifierSpellData : SpellData
    {
    
        [field: PropertySpace(5)]
        [field: Header("Modifier Settings")]
        [field: SerializeField, Required] public ModifierSpell ModifierSpellPrefab { get; private set; }
        
        [field: SerializeField, Tooltip("Used for any interpretation for an int value for the spell")] 
        public int FlatIntValue { get; private set; } = 10;

        [field: SerializeField, Tooltip("Used for any interpretation for a multiplier value for the spell")]
        public float MultiplierValue { get; private set; } = 1.5f;
    }
}
