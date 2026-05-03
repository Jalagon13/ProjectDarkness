using UnityEngine;

namespace ProjectDarkness
{
    public class Npc : MonoBehaviour
    {
        [field:SerializeField]
        public NpcData Data { get; private set; }
        private float _currentHealth;

        public float CurrentHealth => _currentHealth;

        private void Awake()
        {
            _currentHealth = Data.BaseHealth;
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
                Die();
            }
        }

        private void Die()
        {
            if (!Data.CanDie)
            {
                return; // HP reached 0, but the NPC is set to not die
            }
            Debug.Log($"Npc {gameObject.name} died");
            Destroy(gameObject);
        }    
    }
}
