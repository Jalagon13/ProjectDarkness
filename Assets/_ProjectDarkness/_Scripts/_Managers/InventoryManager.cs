using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectDarkness
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }

        public event Action OnInventoryUpdated;

        [SerializeField, Min(1), Tooltip("Total number of inventory slots available to the player")]
        private int _slotCount;
        [field: SerializeField] public InventoryUI InventoryUI { get; private set; }

        [Header("Starting Items")]
        [SerializeField] private float _initialDelay;
        [SerializeField] private float _delayBetweenItemsGiven;
        [SerializeField] private List<InventorySlot> _startingItems = new();

        private readonly List<InventorySlot> _slots = new();
        public List<InventorySlot> Slots => _slots;
        public bool IsFull => !_slots.Exists(slot => slot.HasSpell == false);

        private void Awake()
        {
            Instance = this;
            InitializeSlots();
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(_initialDelay);

            foreach (InventorySlot slotItem in _startingItems)
            {
                AddSpell(slotItem.SpellData);
                yield return new WaitForSeconds(_delayBetweenItemsGiven);
            }
        }

        public void AddSpell(SpellData spellData)
        {
            if (spellData == null || IsFull)
            {
                return;
            }
            
            foreach (InventorySlot slot in _slots)
            {
                if (slot.HasSpell)
                {
                    continue;
                }

                slot.SetSpell(spellData);
                OnInventoryUpdated?.Invoke();
                return;
            }
        }

        public InventorySlot GetSlot(int slotIndex)
        {
            return IsValidSlotIndex(slotIndex) ? _slots[slotIndex] : new InventorySlot();
        }

        private bool IsValidSlotIndex(int slotIndex)
        {
            return slotIndex >= 0 && slotIndex < _slots.Count;
        }

        private void InitializeSlots()
        {
            _slots.Clear();

            for (int i = 0; i < _slotCount; i++)
            {
                _slots.Add(new InventorySlot());
            }
        }
    }
}
