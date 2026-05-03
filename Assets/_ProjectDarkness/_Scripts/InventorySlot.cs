using System;
using UnityEngine;

namespace ProjectDarkness
{
    [Serializable]
    public class InventorySlot
    {
        [SerializeField] private SpellData _spellData;

        public SpellData SpellData => _spellData;
        public bool HasSpell => _spellData != null;

        public void SetSpell(SpellData spellData)
        {
            _spellData = spellData;
        }

        public void Clear()
        {
            _spellData = null;
        }
    }
}
