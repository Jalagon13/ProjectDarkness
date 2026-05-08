using UnityEngine;

namespace ProjectDarkness
{
    public class DamageCollider : MonoBehaviour
    {
        private ProjectileSpell _spell;
    
        private void Awake()
        {
            if(transform.root.TryGetComponent(out ProjectileSpell spell))
            {
                _spell = spell;
            }
        }
    
        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent(out Npc npc))
            {
                Debug.Log($"Found NPC {npc.gameObject.name}");
                npc.TakeDamage(_spell.RuntimeData.Damage);
            }
        }
    }
}
