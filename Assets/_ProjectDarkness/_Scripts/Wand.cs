using UnityEngine;
using System.Collections.Generic;
using System;
using Sirenix.OdinInspector;

namespace ProjectDarkness
{
    public class Wand : MonoBehaviour
    {
        public event Action OnManaUpdated;

        [SerializeField, Required] 
        private WandData _wandData;
        public WandData WandData => _wandData;
        
        [field: SerializeField] 
        public Transform CastPoint { get; private set; }
        
        [SerializeField] 
        private List<InventorySlot> _spellInventory = new();
        public List<InventorySlot> SpellInventory => _spellInventory;

        private float _aimRayDistance = 1000f;
        
        private float _currentMana;
        public float CurrentMana => _currentMana;
        
        private Timer _castDelayTimer;
        
        private Timer _cooldownTimer;
        public Timer CooldownTimer => _cooldownTimer;
        
        private int _currentSequenceIndex;


        private void Awake()
        {
            _castDelayTimer = new Timer(_wandData.CastDelayTime);
            _cooldownTimer = new Timer(_wandData.CooldownTime);
            _currentMana = _wandData != null ? _wandData.ManaAmount : 0f;
            
            SyncInventoryCapacity();
            ResetCastingState();
            
            OnManaUpdated?.Invoke();
        }

        private void OnValidate()
        {
            SyncInventoryCapacity();
        }

        private void Update()
        {
            // Update Mana regneration
            UpdateCurrentMana(_currentMana + (_wandData.ManaRegenPerSec * Time.deltaTime));

            // Update Timers
            _castDelayTimer?.Tick(Time.deltaTime);
            _cooldownTimer?.Tick(Time.deltaTime);

            if (CanCast())
            {
                TryCastCurrentSpell();
            }
        }

        private bool CanCast()
        {
            if (!GameInput.Instance.IsHoldingDownCastSpell) return false;
            if (InventoryManager.Instance.InventoryUI.IsOpen) return false;
            if (LevelManager.Instance.IsTransitioning) return false;
            if (Time.timeScale == 0f) return false;
            if (HealthManager.Instance.IsDead) return false;
            if (GameManager.Instance != null && GameManager.Instance.GameComplete) return false;
            if ((_cooldownTimer?.IsRunning() ?? false) || (_castDelayTimer?.IsRunning() ?? false)) return false;

            return true;
        }

        private void TryCastCurrentSpell()
        {
            // Try to find a valid spell index with a spell
            int spellIndex = GetNextOccupiedSpellIndex(_currentSequenceIndex);
            if (spellIndex < 0)
            {
                if (_currentSequenceIndex > 0)
                {
                    StartCooldown();
                }
                return;
            }

            // Valid spell index found
            SpellData spellData = _spellInventory[spellIndex].SpellData;
            
            if (_currentMana < spellData.ManaDrain)
            {
                return;
            }

            CastSpell(spellData);
            UpdateCurrentMana(_currentMana - spellData.ManaDrain);
            AdvanceSequence(spellIndex);
        }

        private void CastSpell(SpellData spellData)
        {
            Camera mainCamera = Camera.main;

            Ray aimRay = new(mainCamera.transform.position, mainCamera.transform.forward);
            Vector3 targetPoint = aimRay.origin + (aimRay.direction * _aimRayDistance);

            if (Physics.Raycast(aimRay, out RaycastHit hit, _aimRayDistance))
            {
                targetPoint = hit.point;
            }

            Vector3 projectileDirection = (targetPoint - CastPoint.position).normalized;
            if (projectileDirection.sqrMagnitude <= Mathf.Epsilon)
            {
                projectileDirection = CastPoint.forward;
            }

            Spell spell = Instantiate(spellData.SpellPrefab, CastPoint.position, Quaternion.LookRotation(projectileDirection));

            spell.Cast(projectileDirection);
        }

        private void AdvanceSequence(int currentSpellIndex)
        {
            _currentSequenceIndex = currentSpellIndex + 1;

            if (GetNextOccupiedSpellIndex(_currentSequenceIndex) >= 0)
            {
                _castDelayTimer.StartTimer();
                return;
            }

            StartCooldown();
        }

        private int GetNextOccupiedSpellIndex(int startIndex)
        {
            for (int i = Mathf.Max(0, startIndex); i < _spellInventory.Count; i++)
            {
                if (_spellInventory[i] != null && _spellInventory[i].HasSpell)
                {
                    return i;
                }
            }

            return -1;
        }

        private void StartCooldown()
        {
            _cooldownTimer.StartTimer();
            _currentSequenceIndex = 0;
            _castDelayTimer.StopTimer();
        }

        private void ResetCastingState()
        {
            _castDelayTimer.StopTimer();
            _cooldownTimer.StopTimer();
            _currentSequenceIndex = 0;
        }

        private void SyncInventoryCapacity()
        {
            _spellInventory ??= new List<InventorySlot>();

            int targetCapacity = Mathf.Max(0, _wandData.Capacity);

            while (_spellInventory.Count < targetCapacity)
            {
                _spellInventory.Add(new InventorySlot());
            }

            while (_spellInventory.Count > targetCapacity)
            {
                _spellInventory.RemoveAt(_spellInventory.Count - 1);
            }
        }

        private bool IsValidSlotIndex(int slotIndex)
        {
            return slotIndex >= 0 && slotIndex < _spellInventory.Count;
        }

        private void UpdateCurrentMana(float newManaAmount)
        {
            float clampedManaAmount = Mathf.Clamp(newManaAmount, 0f, _wandData.ManaAmount);

            if (Mathf.Approximately(_currentMana, clampedManaAmount))
            {
                return;
            }

            _currentMana = clampedManaAmount;
            OnManaUpdated?.Invoke();
        }
    }
}
