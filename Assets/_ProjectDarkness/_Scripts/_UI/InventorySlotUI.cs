using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ProjectDarkness
{
    public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        [SerializeField] private Image _iconImage;

        private Transform _dragRoot;
        private Action _onSlotChanged;
        private static InventorySlotUI _currentlyDraggedSlot;
        private Transform _originalParent;
        private InventorySlot _assignedSlot;

        public int SlotIndex { get; private set; }

        public void Initialize(Transform dragRoot, int slotIndex, System.Action onSlotChanged)
        {
            SlotIndex = slotIndex;
            _dragRoot = dragRoot;
            _onSlotChanged = onSlotChanged;
            name = $"Inventory Slot {slotIndex + 1}";
        }

        public void Refresh(InventorySlot stack)
        {
            _assignedSlot = stack;
            bool showItem = stack != null && stack.HasSpell;
            _iconImage.enabled = showItem && stack.SpellData.UiDisplay != null;
            _iconImage.sprite = showItem ? stack.SpellData.UiDisplay : null;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_assignedSlot == null || !_assignedSlot.HasSpell) return;

            _currentlyDraggedSlot = this;
            _originalParent = _iconImage.transform.parent;

            _iconImage.transform.SetParent(_dragRoot);
            _iconImage.transform.SetAsLastSibling();
            _iconImage.raycastTarget = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_currentlyDraggedSlot != this) return;

            _iconImage.transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_currentlyDraggedSlot != this) return;

            _iconImage.transform.SetParent(_originalParent);
            _iconImage.rectTransform.anchoredPosition = Vector2.zero;
            _iconImage.raycastTarget = true;

            _currentlyDraggedSlot = null;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (_currentlyDraggedSlot != null && _currentlyDraggedSlot != this)
            {
                SpellData temp = this._assignedSlot.SpellData;
                this._assignedSlot.SetSpell(_currentlyDraggedSlot._assignedSlot.SpellData);
                _currentlyDraggedSlot._assignedSlot.SetSpell(temp);

                this._onSlotChanged?.Invoke();
                if (_currentlyDraggedSlot._onSlotChanged != this._onSlotChanged)
                {
                    _currentlyDraggedSlot._onSlotChanged?.Invoke();
                }
            }
        }
    }
}