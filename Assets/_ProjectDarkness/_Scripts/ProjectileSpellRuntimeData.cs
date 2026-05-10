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

        public ProjectileSpellRuntimeData(SpellBlock spellBlock)
        {
            Speed = spellBlock.ProjectileSpell.Speed;
            Lifetime = spellBlock.ProjectileSpell.Lifetime;
            Scatter = spellBlock.ProjectileSpell.Scatter;
            Damage = spellBlock.ProjectileSpell.Damage;
            PierceCount = spellBlock.ProjectileSpell.PierceCount;
            BounceCount = spellBlock.ProjectileSpell.BounceCount;
            Knockback = spellBlock.ProjectileSpell.Knockback;
        }

        public void AddSpeed(float value)
        {
            Speed = Mathf.Max(0f, Speed + value);
        }

        public void MultiplySpeed(float value)
        {
            Speed = Mathf.Max(0f, Speed * value);
        }

        public void AddLifetime(float value)
        {
            Lifetime = Mathf.Max(0f, Lifetime + value);
        }

        public void MultiplyLifetime(float value)
        {
            Lifetime = Mathf.Max(0f, Lifetime * value);
        }

        public void AddScatter(float value)
        {
            Scatter = Mathf.Max(0f, Scatter + value);
        }

        public void MultiplyScatter(float value)
        {
            Scatter = Mathf.Max(0f, Scatter * value);
        }

        public void AddDamage(int value)
        {
            Damage = Mathf.Max(0, Damage + value);
        }

        public void MultiplyDamage(float value)
        {
            Damage = Mathf.Max(0, Mathf.RoundToInt(Damage * value));
        }

        public void AddPierceCount(int value)
        {
            PierceCount = Mathf.Max(0, PierceCount + value);
        }

        public void MultiplyPierceCount(float value)
        {
            PierceCount = Mathf.Max(0, Mathf.RoundToInt(PierceCount * value));
        }

        public void AddBounceCount(int value)
        {
            BounceCount = Mathf.Max(0, BounceCount + value);
        }

        public void MultiplyBounceCount(float value)
        {
            BounceCount = Mathf.Max(0, Mathf.RoundToInt(BounceCount * value));
        }

        public void AddKnockback(int value)
        {
            Knockback = Mathf.Max(0, Knockback + value);
        }

        public void MultiplyKnockback(float value)
        {
            Knockback = Mathf.Max(0, Mathf.RoundToInt(Knockback * value));
        }
    }
}
