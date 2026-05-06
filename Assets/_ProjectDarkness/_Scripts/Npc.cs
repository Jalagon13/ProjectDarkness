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
            // Apply defense from Data, ensuring damage doesn't go below 1
            float finalDamage = Mathf.Max(1f, damageAmount - Data.BaseDefense);
            Debug.Log($"Npc {gameObject.name} damaged by {finalDamage}");
            _currentHealth -= finalDamage;

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
    }
}
