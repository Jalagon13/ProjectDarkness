using UnityEngine;

namespace ProjectDarkness
{
    public abstract class ModifierSpell : MonoBehaviour
    {
        public abstract void ModifySpell(ProjectileSpell spell, ModifierSpellData modifierData);
    }
}
