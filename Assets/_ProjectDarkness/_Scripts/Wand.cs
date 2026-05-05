using UnityEngine;
using System.Collections.Generic;
using System;

namespace ProjectDarkness
{
    public class Wand : MonoBehaviour
    {
        public event Action OnManaUpdated;

        [SerializeField] private WandData _wandData;
        public WandData WandData => _wandData;
        
        [field: SerializeField] public Transform CastPoint { get; private set; }
        [SerializeField] private List<InventorySlot> _spellInventory = new();
        public List<InventorySlot> SpellInventory => _spellInventory;

        private float _aimRayDistance = 1000f;
        private float _currentMana;
        private Timer _castDelayTimer;
        private Timer _cooldownTimer;
        public Timer CooldownTimer => _cooldownTimer;
        private int _currentSequenceIndex;

        public float CurrentMana => _currentMana;

        private void Awake()
        {
            SyncInventoryCapacity();
            InitializeTimers();
            ResetCastingState();
            _currentMana = _wandData != null ? _wandData.ManaAmount : 0f;
            NotifyManaUpdated();
        }

        private void OnValidate()
        {
            SyncInventoryCapacity();
        }

        private void Update()
        {
            RegenerateMana();
            UpdateTimers();

            if (_wandData == null || CastPoint == null)
            {
                return;
            }

            if (!GameInput.Instance.IsHoldingDownCastSpell || InventoryManager.Instance.InventoryUI.IsOpen || LevelManager.Instance.IsTransitioning)
            {
                return;
            }

            if ((_cooldownTimer?.IsRunning() ?? false) || (_castDelayTimer?.IsRunning() ?? false))
            {
                return;
            }

            TryCastCurrentSpell();
        }

        public bool TrySetSpellAtSlot(int slotIndex, SpellData spellData)
        {
            if (!IsValidSlotIndex(slotIndex))
            {
                return false;
            }

            _spellInventory[slotIndex].SetSpell(spellData);
            return true;
        }

        public bool TryClearSpellAtSlot(int slotIndex)
        {
            if (!IsValidSlotIndex(slotIndex))
            {
                return false;
            }

            _spellInventory[slotIndex].Clear();
            return true;
        }

        private void TryCastCurrentSpell()
        {
            int spellIndex = GetNextOccupiedSpellIndex(_currentSequenceIndex);
            if (spellIndex < 0)
            {
                if (_currentSequenceIndex > 0)
                {
                    StartCooldown();
                }
                return;
            }

            SpellData spellData = _spellInventory[spellIndex].SpellData;
            if (spellData == null || spellData.SpellPrefab == null)
            {
                AdvanceSequence(spellIndex);
                return;
            }

            if (_currentMana < spellData.ManaReq)
            {
                return;
            }

            if (!CastSpell(spellData))
            {
                return;
            }

            UpdateCurrentMana(_currentMana - spellData.ManaReq);
            AdvanceSequence(spellIndex);
        }

        private bool CastSpell(SpellData spellData)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                return false;
            }

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

            Spell spell = Instantiate(
                spellData.SpellPrefab,
                CastPoint.position,
                Quaternion.LookRotation(projectileDirection));

            spell.Cast(projectileDirection);
            return true;
        }

        private void AdvanceSequence(int currentSpellIndex)
        {
            _currentSequenceIndex = currentSpellIndex + 1;

            if (GetNextOccupiedSpellIndex(_currentSequenceIndex) >= 0)
            {
                StartTimer(_castDelayTimer);
                return;
            }

            StartCooldown();
        }

        private void StartCooldown()
        {
            StartTimer(_cooldownTimer);
            _currentSequenceIndex = 0;
            StopTimer(_castDelayTimer);
        }

        private void ResetCastingState()
        {
            StopTimer(_castDelayTimer);
            StopTimer(_cooldownTimer);
            _currentSequenceIndex = 0;
        }

        private void RegenerateMana()
        {
            if (_wandData == null)
            {
                return;
            }

            UpdateCurrentMana(_currentMana + (_wandData.ManaRegenRatePerSec * Time.deltaTime));
        }

        private void UpdateTimers()
        {
            _castDelayTimer?.Tick(Time.deltaTime);
            _cooldownTimer?.Tick(Time.deltaTime);
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

        private void SyncInventoryCapacity()
        {
            if (_spellInventory == null)
            {
                _spellInventory = new List<InventorySlot>();
            }

            int targetCapacity = _wandData != null ? Mathf.Max(0, _wandData.Capacity) : _spellInventory.Count;

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

        private void InitializeTimers()
        {
            float castDelay = _wandData != null ? _wandData.CastDelayTime : 0f;
            float cooldown = _wandData != null ? _wandData.CooldownTime : 0f;

            _castDelayTimer = new Timer(castDelay);
            _cooldownTimer = new Timer(cooldown);
        }

        private static void StartTimer(Timer timer)
        {
            if (timer == null)
            {
                return;
            }

            timer.Reset();
            timer.IsPaused = false;
        }

        private static void StopTimer(Timer timer)
        {
            if (timer == null)
            {
                return;
            }

            timer.RemainingSeconds = 0f;
            timer.IsPaused = false;
        }

        private void UpdateCurrentMana(float newManaAmount)
        {
            float clampedManaAmount = _wandData != null
                ? Mathf.Clamp(newManaAmount, 0f, _wandData.ManaAmount)
                : Mathf.Max(0f, newManaAmount);

            if (Mathf.Approximately(_currentMana, clampedManaAmount))
            {
                return;
            }

            _currentMana = clampedManaAmount;
            NotifyManaUpdated();
        }

        private void NotifyManaUpdated()
        {
            OnManaUpdated?.Invoke();
        }

    }
}
