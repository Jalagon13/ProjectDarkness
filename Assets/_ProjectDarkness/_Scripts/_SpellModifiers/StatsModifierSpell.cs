using UnityEngine;

namespace ProjectDarkness
{
    public class StatsModifierSpell : ModifierSpell
    {
        [SerializeField]
        private TargetStat _targetStat;

        [SerializeField]
        private StatModificationMode _modificationMode = StatModificationMode.FlatAdd;

        public override void ModifySpell(ProjectileSpell spell, ModifierSpellData modifierData)
        {
            switch (_modificationMode)
            {
                case StatModificationMode.FlatAdd:
                    ApplyFlatModification(spell.RuntimeData, modifierData);
                    break;
                case StatModificationMode.Multiply:
                    ApplyMultiplierModification(spell.RuntimeData, modifierData);
                    break;
            }
        }

        private void ApplyFlatModification(ProjectileSpellRuntimeData runtimeData, ModifierSpellData modifierData)
        {
            switch (_targetStat)
            {
                case TargetStat.Speed:
                    runtimeData.AddSpeed(modifierData.FlatIntValue);
                    break;
                case TargetStat.Lifetime:
                    runtimeData.AddLifetime(modifierData.FlatIntValue);
                    break;
                case TargetStat.Scatter:
                    runtimeData.AddScatter(modifierData.FlatIntValue);
                    break;
                case TargetStat.Damage:
                    runtimeData.AddDamage(modifierData.FlatIntValue);
                    break;
                case TargetStat.PierceCount:
                    runtimeData.AddPierceCount(modifierData.FlatIntValue);
                    break;
                case TargetStat.BounceCount:
                    runtimeData.AddBounceCount(modifierData.FlatIntValue);
                    break;
                case TargetStat.Knockback:
                    runtimeData.AddKnockback(modifierData.FlatIntValue);
                    break;
            }
        }

        private void ApplyMultiplierModification(ProjectileSpellRuntimeData runtimeData, ModifierSpellData modifierData)
        {
            switch (_targetStat)
            {
                case TargetStat.Speed:
                    runtimeData.MultiplySpeed(modifierData.MultiplierValue);
                    break;
                case TargetStat.Lifetime:
                    runtimeData.MultiplyLifetime(modifierData.MultiplierValue);
                    break;
                case TargetStat.Scatter:
                    runtimeData.MultiplyScatter(modifierData.MultiplierValue);
                    break;
                case TargetStat.Damage:
                    runtimeData.MultiplyDamage(modifierData.MultiplierValue);
                    break;
                case TargetStat.PierceCount:
                    runtimeData.MultiplyPierceCount(modifierData.MultiplierValue);
                    break;
                case TargetStat.BounceCount:
                    runtimeData.MultiplyBounceCount(modifierData.MultiplierValue);
                    break;
                case TargetStat.Knockback:
                    runtimeData.MultiplyKnockback(modifierData.MultiplierValue);
                    break;
            }
        }

        private enum TargetStat
        {
            Speed,
            Lifetime,
            Scatter,
            Damage,
            PierceCount,
            BounceCount,
            Knockback
        }

        private enum StatModificationMode
        {
            FlatAdd,
            Multiply
        }
    }
}
