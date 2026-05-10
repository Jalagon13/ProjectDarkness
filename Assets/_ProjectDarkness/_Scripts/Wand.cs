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
        
        private 

        private readonly float _aimRayDistance = 1000f;
        
        private float _currentMana;
        public float CurrentMana => _currentMana;
        
        private Timer _spellChargeTimer;
        public Timer SpellChargeTimer => _spellChargeTimer;
        
        private Timer _cooldownTimer;
        public Timer CooldownTimer => _cooldownTimer;
        
        private int _currentSequenceIndex;


        private void Awake()
        {
            _spellChargeTimer = new Timer(_wandData.SpellChargeTime);
            _spellChargeTimer.OnTimerEnd += SpellChargeTimer_OnTimerEnd;
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

        private void OnDestroy()
        {
            if (_spellChargeTimer != null)
            {
                _spellChargeTimer.OnTimerEnd -= SpellChargeTimer_OnTimerEnd;
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
            int spellIndex = GetNextProjectileSpellIndex(_currentSequenceIndex);
            if (spellIndex < 0 || (_spellChargeTimer?.IsRunning() ?? false))
            {
                return false;
            }

            ProjectileSpellData projectileSpellData = _spellInventory[spellIndex].SpellData as ProjectileSpellData;
            CastContext castContext = BuildCastContextForSpell(spellIndex, projectileSpellData);

            if (_currentMana < castContext.GetTotalManaCost())
            {
                return false;
            }

            return true;
        }

        private void StartSpellCharge()
        {
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
            int spellIndex = GetNextProjectileSpellIndex(_currentSequenceIndex);
            if (spellIndex < 0)
            {
                if (_currentSequenceIndex > 0)
                {
                    StartCooldown();
                }
                return;
            }

            ProjectileSpellData projectileSpellData = _spellInventory[spellIndex].SpellData as ProjectileSpellData;
            CastContext castContext = BuildCastContextForSpell(spellIndex, projectileSpellData);

            if (_currentMana < castContext.GetTotalManaCost())
            {
                return;
            }

            CastSpell(castContext);
            UpdateCurrentMana(_currentMana - castContext.GetTotalManaCost());
            AdvanceSequence(spellIndex);
        }

        private CastContext BuildCastContextForSpell(int spellIndex, ProjectileSpellData mainProjectileSpell)
        {
            List<ModifierSpellData> modifiers = new();
        
            for(int i = 0; i < spellIndex; i++)
            {
                if(_spellInventory[i].HasSpell && _spellInventory[i].SpellData is ModifierSpellData modifier)
                {
                    modifiers.Add(modifier);
                }
            }
            
            return new CastContext(modifiers, mainProjectileSpell);
        }

        private void CastSpell(CastContext castContext)
        {
            Vector3 projectileDirection = CalculateProjectileDirection(castContext);

            ProjectileSpell spell = Instantiate(castContext.ProjectileSpell.ProjectileSpellPrefab, CastPoint.position, Quaternion.LookRotation(projectileDirection));
            spell.Initialize(castContext);
            spell.Cast(projectileDirection);
        }
        
        private Vector3 CalculateProjectileDirection(CastContext castContext)
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

            float totalScatter = Mathf.Max(0f, _wandData.Scatter + castContext.ProjectileSpell.Scatter);
            if (totalScatter <= 0f)
            {
                return projectileDirection;
            }

            Vector2 scatterOffset = UnityEngine.Random.insideUnitCircle * totalScatter;
            Quaternion baseRotation = Quaternion.LookRotation(projectileDirection);
            Quaternion localSpread = Quaternion.AngleAxis(scatterOffset.x, baseRotation * Vector3.up) * Quaternion.AngleAxis(scatterOffset.y, baseRotation * Vector3.right);

            return (localSpread * projectileDirection).normalized;
        }

        private void AdvanceSequence(int currentSpellIndex)
        {
            _currentSequenceIndex = currentSpellIndex + 1;

            if (GetNextProjectileSpellIndex(_currentSequenceIndex) < 0)
            {
                StartCooldown();
                return;
            }
        }

        private int GetNextProjectileSpellIndex(int startIndex)
        {
            for (int i = Mathf.Max(0, startIndex); i < _spellInventory.Count; i++)
            {
                if (_spellInventory[i] != null && _spellInventory[i].HasSpell && _spellInventory[i].SpellData is ProjectileSpellData)
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
            _spellChargeTimer.StopTimer();
        }

        private void ResetCastingState()
        {
            _spellChargeTimer.StopTimer();
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
