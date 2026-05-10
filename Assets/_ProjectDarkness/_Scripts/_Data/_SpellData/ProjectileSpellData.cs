using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectDarkness
{
    [CreateAssetMenu(fileName = "New Projectile Spell Data", menuName = "ProjectDarkness/SpellData/Projectile")]
    public class ProjectileSpellData : SpellData
    {
        [field: SerializeField, Required] public ProjectileSpell ProjectileSpellPrefab { get; private set; }

        [field: Header("Projectile Stats")]
        [field: SerializeField] public float Speed { get; private set; } = 10f;
        [field: SerializeField] public float Lifetime { get; private set; } = 10f;
        [field: SerializeField] public float Scatter { get; private set; } = 0f;

        [field: PropertySpace(5)]
        [field: Header("Combat Stats")]
        [field: SerializeField] public int Damage { get; private set; } = 5;
        [field: SerializeField] public int PierceCount { get; private set; } = 0;
        [field: SerializeField] public int BounceCount { get; private set; } = 0;
        [field: SerializeField] public int Knockback { get; private set; } = 3;
    }
}
