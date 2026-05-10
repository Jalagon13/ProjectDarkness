using System.Collections.Generic;
using UnityEngine;

namespace ProjectDarkness
{
    public class SpellBlock 
    {
        public List<ModifierSpellData> Modifiers { get; private set; }
        public ProjectileSpellData ProjectileSpell { get; private set; }
        public float TotalSpellChargeTime { get; private set; }
        
        public SpellBlock(List<ModifierSpellData> modifiers, ProjectileSpellData mainProjectileSpell, float wandSpellChargeTime)
        {
            Modifiers = modifiers ?? new List<ModifierSpellData>();
            ProjectileSpell = mainProjectileSpell;

            TotalSpellChargeTime = wandSpellChargeTime;
            if (ProjectileSpell != null)
            {
                TotalSpellChargeTime += ProjectileSpell.SpellChargeTime;
            }
            
            foreach (ModifierSpellData mod in Modifiers)
            {
                TotalSpellChargeTime += mod.SpellChargeTime;
            }
        }
        
        public int GetTotalManaCost()
        {
            int totalMana = 0;
            
            if (ProjectileSpell != null)
            {
                totalMana += ProjectileSpell.ManaDrain;
            }
        
            foreach (ModifierSpellData item in Modifiers)
            {
                totalMana += item.ManaDrain;
            }
            
            return totalMana;
        }
    }
}