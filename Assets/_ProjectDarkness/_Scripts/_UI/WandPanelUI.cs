using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectDarkness
{
    public class WandPanelUI : MonoBehaviour
    {
        [SerializeField] private InventorySlotUI _inventorySlotUIPrefab;
        [SerializeField] private RectTransform _inventoryPanel;
        
        private Wand _wand;
        private System.Action _onInventoryUpdated;
        private readonly List<InventorySlotUI> _slotUis = new();

        public void Initialize(Wand wand, Transform dragRoot, Action onInventoryUpdated)
        {
            _wand = wand;
            _onInventoryUpdated = onInventoryUpdated;

            for (int i = 0; i < wand.SpellInventory.Count; i++)
            {
                InventorySlotUI slotUi = Instantiate(_inventorySlotUIPrefab, _inventoryPanel);
                slotUi.transform.localScale = Vector3.one;
                slotUi.Initialize(dragRoot, i, RefreshAllAndNotify);
                _slotUis.Add(slotUi);
            }
            
            RefreshAll();
        }

        private void RefreshAllAndNotify()
        {
            RefreshAll();
            _onInventoryUpdated?.Invoke();
        }

        public void RefreshAll()
        {
            for (int i = 0; i < _slotUis.Count; i++)
            {
                _slotUis[i].Refresh(_wand.SpellInventory[i]);
            }
        }
    }
}
