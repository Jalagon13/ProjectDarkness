using System;
using UnityEngine.AI;
using UnityEngine;

namespace ProjectDarkness
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Npc : MonoBehaviour
    {
        public event EventHandler<EventArgs> OnHealthUpdated;
        public event EventHandler<EventArgs> OnDeath;
    
        [field:SerializeField] 
        public NpcData Data { get; private set; }

        [Header("NPC Navigation")]
        [SerializeField] 
        private float _stoppingDistance = 0.75f;

        private float _currentHealth;
        private bool _isAiEnabled;
        private NavMeshAgent _navMeshAgent;
        private Rigidbody _rigidbody;

        public float CurrentHealth => _currentHealth;
        public bool IsAiEnabled => _isAiEnabled;
        protected NavMeshAgent NavMeshAgent => _navMeshAgent;
        protected Rigidbody Rigidbody => _rigidbody;

        protected virtual void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _rigidbody = GetComponent<Rigidbody>();

            _currentHealth = Data.BaseHealth;

            _navMeshAgent.speed = Data.BaseSpeed;
            _navMeshAgent.stoppingDistance = _stoppingDistance;
            _navMeshAgent.angularSpeed = 0f;
            _navMeshAgent.enabled = false;

            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            
            if(Data.IsCombatNpc && transform.root.TryGetComponent(out CombatRoom combatRoom))
            {
                combatRoom.RegisterCombatNpc(this);
            }
        }

        public virtual void SetAiEnabled(bool isEnabled)
        {
            if (_isAiEnabled == isEnabled)
            {
                return;
            }

            _isAiEnabled = isEnabled;

            if (_isAiEnabled)
            {
                OnAiEnabled();
            }
            else
            {
                OnAiDisabled();
            }
        }

        protected virtual void OnAiEnabled()
        {
            _navMeshAgent.enabled = true;
            _navMeshAgent.speed = Data.BaseSpeed;
            _navMeshAgent.stoppingDistance = _stoppingDistance;
            _navMeshAgent.ResetPath();
            _navMeshAgent.Warp(transform.position);
        }

        protected virtual void OnAiDisabled()
        {
            if (!_navMeshAgent.enabled)
            {
                return;
            }

            _navMeshAgent.ResetPath();
            _navMeshAgent.enabled = false;
        }

        public void TakeDamage(float damageAmount)
        {
            TakeDamage(damageAmount, GetDefaultDamagePopupPosition());
        }

        public void TakeDamage(float damageAmount, Vector3 damageWorldPosition)
        {
            // Apply defense from Data, ensuring damage doesn't go below 1
            float finalDamage = Mathf.Max(1f, damageAmount - Data.BaseDefense);
            
            _currentHealth -= finalDamage;

            DamagePopupManager.Instance?.ShowDamagePopup(Mathf.RoundToInt(finalDamage), damageWorldPosition);

            if (_currentHealth <= 0f)
            {
                _currentHealth = 0f;
                OnHealthUpdated?.Invoke(this, EventArgs.Empty);
                Die();
            }
            else
            {
                OnHealthUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        private void Die()
        {
            if (!Data.CanDie)
            {
                return; // HP reached 0, but the NPC is set to not die
            }
            Debug.Log($"Npc {gameObject.name} died");
            OnDeath?.Invoke(this, EventArgs.Empty);
            Destroy(gameObject);
        }

        private Vector3 GetDefaultDamagePopupPosition()
        {
            if (TryGetComponent(out Collider npcCollider))
            {
                return npcCollider.bounds.center;
            }

            return transform.position + Vector3.up;
        }
    }
}
