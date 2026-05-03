using UnityEngine;
using UnityEngine.UI;

namespace ProjectDarkness
{
    [CreateAssetMenu(fileName = "New Spell Data", menuName = "ProjectDarkness/SpellData")]
    public class SpellData : ScriptableObject
    {
        [field: SerializeField] public Spell SpellPrefab { get; private set; }
        [field: SerializeField] public Sprite UiDisplay { get; private set; }
        [field: SerializeField] public float Speed { get; private set; } = 10f;
        [field: SerializeField] public float Distance { get; private set; } = 10f;
        [field: SerializeField] public int ManaReq { get; private set; } = 10;
    }
}
