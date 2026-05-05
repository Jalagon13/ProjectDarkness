using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectDarkness
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private InventorySlotUI _slotPrefab;
        [SerializeField] private RectTransform _inventoryPanel;
        [SerializeField] private RectTransform _inventoryMenu;
        [Header("Wand UI Settings")]
        [SerializeField] private RectTransform _wandPanel;
        [SerializeField] private WandPanelUI _wandPanelUIPrefab;
        [SerializeField] private List<Wand> _wandList;

        private readonly List<InventorySlotUI> _slotUis = new();
        private readonly List<WandPanelUI> _wandPanelUis = new();
        private bool _isOpen;
        public bool IsOpen => _isOpen;



        private void Start()
        {
            InventoryManager.Instance.OnInventoryUpdated += RefreshAll;
            GameInput.Instance.OnToggleInventory += ToggleInventory;
            GameInput.Instance.OnTogglePauseMenu += OnTogglePauseMenu;

            BuildSlots();
            BuildWandPanels();
            RefreshAll();

            _isOpen = false;
            Hide();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        private void OnDestroy()
        {
            InventoryManager.Instance.OnInventoryUpdated -= RefreshAll;
            GameInput.Instance.OnToggleInventory -= ToggleInventory;
            GameInput.Instance.OnTogglePauseMenu -= OnTogglePauseMenu;
        }

        private void OnTogglePauseMenu()
        {
            if(_isOpen)
            {
                ToggleInventory();
            }
        }

        private void ToggleInventory()
        {
            _isOpen = !_isOpen;
            
            if (_isOpen)
            {
                Show();
                RefreshAll();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Hide();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void BuildSlots()
        {
            for (int slotIndex = 0; slotIndex < InventoryManager.Instance.Slots.Count; slotIndex++)
            {
                InventorySlotUI slotUi = CreateSlotInstance();
                slotUi.Initialize(transform, slotIndex, RefreshAll);
                _slotUis.Add(slotUi);
            }
        }

        private void BuildWandPanels()
        {
            foreach (Wand wand in _wandList)
            {
                if (wand == null) continue;
                WandPanelUI wandPanel = Instantiate(_wandPanelUIPrefab, _wandPanel);
                wandPanel.transform.localScale = Vector3.one;
                wandPanel.Initialize(wand, transform, RefreshAll);
                _wandPanelUis.Add(wandPanel);
            }
        }

        private void RefreshAll()
        {
            foreach (InventorySlotUI slotUi in _slotUis)
            {
                InventorySlot slot = InventoryManager.Instance.GetSlot(slotUi.SlotIndex);
                slotUi.Refresh(slot);
            }

            foreach (WandPanelUI wandPanel in _wandPanelUis)
            {
                wandPanel.RefreshAll();
            }
        }

        private InventorySlotUI CreateSlotInstance()
        {
            InventorySlotUI slot = Instantiate(_slotPrefab, _inventoryPanel);
            slot.transform.localScale = Vector3.one;
            return slot;
        }
        
        private void Show()
        {
            _inventoryMenu.gameObject.SetActive(true);
        }
        
        private void Hide()
        {
            _inventoryMenu.gameObject.SetActive(false);
        }
    }
}
