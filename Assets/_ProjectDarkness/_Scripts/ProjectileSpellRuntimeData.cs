using UnityEngine;

namespace ProjectDarkness
{
    public class ProjectileSpellRuntimeData 
    {
        public float Speed { get; private set; }
        public float Lifetime { get; private set; } 
        public float Scatter { get; private set; }

        public int Damage { get; private set; }
        public int PierceCount { get; private set; }
        public int BounceCount { get; private set; }
        public int Knockback { get; private set; }

        public ProjectileSpellRuntimeData(CastContext castContext)
        {
            Speed = castContext.ProjectileSpell.Speed;
            Lifetime = castContext.ProjectileSpell.Lifetime;
            Scatter = castContext.ProjectileSpell.Scatter;
            Damage = castContext.ProjectileSpell.Damage;
            PierceCount = castContext.ProjectileSpell.PierceCount;
            BounceCount = castContext.ProjectileSpell.BounceCount;
            Knockback = castContext.ProjectileSpell.Knockback;
        }
    }
}