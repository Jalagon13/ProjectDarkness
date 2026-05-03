using UnityEngine;

namespace ProjectDarkness
{
    public class DamageCollider : MonoBehaviour
    {
        private Spell _spell;
    
        private void Awake()
        {
            if(transform.root.TryGetComponent(out Spell spell))
            {
                _spell = spell;
            }
        }
    
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"Collided with {other.gameObject.name}");
            if(other.TryGetComponent(out Npc npc))
            {
                Debug.Log($"Found NPC {npc.gameObject.name}");
                npc.TakeDamage(_spell.Data.Damage);
            }
        }
    }
}
