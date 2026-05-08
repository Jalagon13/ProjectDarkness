using System.Collections.Generic;
using UnityEngine;

namespace ProjectDarkness
{
    public class CastContext 
    {
        private List<ModifierSpellData> _modifiers = new();
        public List<ModifierSpellData> Modifiers => _modifiers;
        
        private ProjectileSpellData _mainProjectileSpell;
        public ProjectileSpellData ProjectileSpell => _mainProjectileSpell;
        
        public CastContext(List<ModifierSpellData> modifiers, ProjectileSpellData mainProjectileSpell)
        {
            _modifiers = modifiers;
            _mainProjectileSpell = mainProjectileSpell;
        }
        
        public int GetTotalManaCost()
        {
            int totalMana = 0;
            
            totalMana += _mainProjectileSpell.ManaDrain;
        
            foreach (ModifierSpellData item in _modifiers)
            {
                totalMana += item.ManaDrain;
            }
            
            return totalMana;
        }
    }
}