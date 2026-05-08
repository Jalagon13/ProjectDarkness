using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectDarkness
{
    public abstract class SpellData : ScriptableObject
    {
        [field: Header("Base Spell Settings")]
        [field: SerializeField] public string SpellName { get; private set; }
        [field: SerializeField] public Sprite UiDisplay { get; private set; }
        [field: SerializeField] public int ManaDrain { get; private set; } = 10;
    }
}
