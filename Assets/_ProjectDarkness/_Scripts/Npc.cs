using System;
using UnityEngine;

namespace ProjectDarkness
{
    public class Npc : MonoBehaviour
    {
        public event EventHandler<EventArgs> OnHealthUpdated;
        public event EventHandler<EventArgs> OnDeath;
    
        [field:SerializeField] public NpcData Data { get; private set; }

        private float _currentHealth;

        public float CurrentHealth => _currentHealth;

        private void Awake()
        {
            _currentHealth = Data.BaseHealth;
            
            if(Data.IsCombatNpc && transform.root.TryGetComponent(out CombatRoom combatRoom))
            {
                combatRoom.RegisterCombatNpc(this);
            }
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
