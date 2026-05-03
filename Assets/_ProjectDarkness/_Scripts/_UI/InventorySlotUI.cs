using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ProjectDarkness
{
    public class InventorySlotUI : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;

        private InventoryUI _inventoryUI;

        public int SlotIndex { get; private set; }

        public void Initialize(InventoryUI inventoryUI, int slotIndex)
        {
            SlotIndex = slotIndex;
            _inventoryUI = inventoryUI;
            name = $"Inventory Slot {slotIndex + 1}";
        }

        public void Refresh(InventorySlot stack)
        {
            bool showItem = stack != null && stack.HasSpell;
            _iconImage.enabled = showItem && stack.SpellData.UiDisplay != null;
            _iconImage.sprite = showItem ? stack.SpellData.UiDisplay : null;
        }
    }
}