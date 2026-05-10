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
        
        private readonly float _aimRayDistance = 1000f;
        
        private float _currentMana;
        public float CurrentMana => _currentMana;
        
        private Timer _spellChargeTimer;
        public Timer SpellChargeTimer => _spellChargeTimer;
        
        private Timer _cooldownTimer;
        public Timer CooldownTimer => _cooldownTimer;
        
        private List<SpellBlock> _spellBlocks = new();
        private int _currentBlockIndex;

        private void Awake()
        {
            _spellChargeTimer = new Timer(_wandData.SpellChargeTime);
            _spellChargeTimer.OnTimerEnd += SpellChargeTimer_OnTimerEnd;
            _cooldownTimer = new Timer(_wandData.CooldownTime);
            _currentMana = _wandData.ManaAmount;
            
            SyncInventoryCapacity();
            CompileSpellBlocks();
            ResetCastingState();
            
            OnManaUpdated?.Invoke();
        }

        private void Start()
        {
            if (_spellInventory != null)
            {
                foreach (InventorySlot slot in _spellInventory)
                {
                    if (slot != null)
                    {
                        slot.OnSlotChanged += CompileSpellBlocks;
                    }
                }
            }
        }

        private void OnValidate()
        {
            SyncInventoryCapacity();
            CompileSpellBlocks();
        }

        private void OnDestroy()
        {
            if (_spellChargeTimer != null)
            {
                _spellChargeTimer.OnTimerEnd -= SpellChargeTimer_OnTimerEnd;
            }

            if (_spellInventory != null)
            {
                foreach (InventorySlot slot in _spellInventory)
                {
                    if (slot != null)
                    {
                        slot.OnSlotChanged -= CompileSpellBlocks;
                    }
                }
            }
        }

        private void Update()
        {
            // Update Mana regneration
            UpdateCurrentMana(_currentMana + (_wandData.ManaRegenPerSec * Time.deltaTime));

            // Update Timers
            _spellChargeTimer?.Tick(Time.deltaTime);
            _cooldownTimer?.Tick(Time.deltaTime);

            if (!CanMaintainSpellCharge())
            {
                CancelSpellCharge();
                return;
            }

            if (ShouldStartSpellCharge())
            {
                StartSpellCharge();
            }
        }

        private bool CanMaintainSpellCharge()
        {
            if (!GameInput.Instance.IsHoldingDownCastSpell) return false;
            if (InventoryManager.Instance.InventoryUI.IsOpen) return false;
            if (LevelManager.Instance.IsTransitioning) return false;
            if (Time.timeScale == 0f) return false;
            if (HealthManager.Instance.IsDead) return false;
            if (GameManager.Instance != null && GameManager.Instance.GameComplete) return false;
            if (_cooldownTimer?.IsRunning() ?? false) return false;

            return true;
        }

        private bool ShouldStartSpellCharge()
        {
            if (_currentBlockIndex >= _spellBlocks.Count || (_spellChargeTimer?.IsRunning() ?? false))
            {
                return false;
            }

            SpellBlock currentBlock = _spellBlocks[_currentBlockIndex];

            if (_currentMana < currentBlock.GetTotalManaCost())
            {
                return false;
            }

            return true;
        }

        private void StartSpellCharge()
        {
            SpellBlock currentBlock = _spellBlocks[_currentBlockIndex];
            _spellChargeTimer.Duration = currentBlock.TotalSpellChargeTime;

            if (_spellChargeTimer.Duration <= 0f)
            {
                TryCastCurrentSpell();
                return;
            }

            _spellChargeTimer.StartTimer();
        }

        private void CancelSpellCharge()
        {
            _spellChargeTimer?.StopTimer();
        }

        private void SpellChargeTimer_OnTimerEnd(object sender, EventArgs e)
        {
            if (!CanMaintainSpellCharge())
            {
                CancelSpellCharge();
                return;
            }

            TryCastCurrentSpell();
        }

        private void TryCastCurrentSpell()
        {
            if (_currentBlockIndex >= _spellBlocks.Count)
            {
                StartCooldown();
                return;
            }

            SpellBlock currentBlock = _spellBlocks[_currentBlockIndex];

            if (_currentMana < currentBlock.GetTotalManaCost())
            {
                return;
            }

            CastSpell(currentBlock);
            UpdateCurrentMana(_currentMana - currentBlock.GetTotalManaCost());
            AdvanceSequence();
        }

        private void CastSpell(SpellBlock spellBlock)
        {
            Vector3 projectileDirection = CalculateProjectileDirection(spellBlock);

            ProjectileSpell spell = Instantiate(spellBlock.ProjectileSpell.ProjectileSpellPrefab, CastPoint.position, Quaternion.LookRotation(projectileDirection));
            spell.Initialize(spellBlock);
            spell.Cast(projectileDirection);
        }
        
        private Vector3 CalculateProjectileDirection(SpellBlock spellBlock)
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

            float totalScatter = Mathf.Max(0f, _wandData.Scatter + spellBlock.ProjectileSpell.Scatter);
            if (totalScatter <= 0f)
            {
                return projectileDirection;
            }

            Vector2 scatterOffset = UnityEngine.Random.insideUnitCircle * totalScatter;
            Quaternion baseRotation = Quaternion.LookRotation(projectileDirection);
            Quaternion localSpread = Quaternion.AngleAxis(scatterOffset.x, baseRotation * Vector3.up) * Quaternion.AngleAxis(scatterOffset.y, baseRotation * Vector3.right);

            return (localSpread * projectileDirection).normalized;
        }

        private void AdvanceSequence()
        {
            _currentBlockIndex++;

            if (_currentBlockIndex >= _spellBlocks.Count)
            {
                StartCooldown();
                return;
            }
        }

        private void StartCooldown()
        {
            _cooldownTimer.StartTimer();
            _currentBlockIndex = 0;
            _spellChargeTimer.StopTimer();
        }

        private void ResetCastingState()
        {
            _spellChargeTimer?.StopTimer();
            _cooldownTimer?.StopTimer();
            _currentBlockIndex = 0;
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

        public void CompileSpellBlocks()
        {
            Debug.Log($"Compiling Spell Blocks for Wand: {_wandData.WandName}");
        
            _spellBlocks.Clear();
            List<ModifierSpellData> currentModifiers = new();

            float wandChargeTime = _wandData.SpellChargeTime;

            foreach (InventorySlot slot in _spellInventory)
            {
                if (slot == null || !slot.HasSpell) continue;

                if (slot.SpellData is ModifierSpellData modifier)
                {
                    currentModifiers.Add(modifier);
                }
                else if (slot.SpellData is ProjectileSpellData projectile)
                {
                    _spellBlocks.Add(new SpellBlock(new List<ModifierSpellData>(currentModifiers), projectile, wandChargeTime));
                    currentModifiers.Clear();
                }
            }
            
            ResetCastingState();
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
