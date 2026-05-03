using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

namespace ProjectDarkness
{
    [CreateAssetMenu(fileName = "New Npc Data", menuName = "ProjectDarkness/NpcData")]
    public class NpcData : ScriptableObject
    {
        [Header("Core Stats")]
        [field: SerializeField, Tooltip("Base HP for character")]
        public int BaseHealth { get; private set; }
        [field: SerializeField, Tooltip("Base Speed for character")]
        public float BaseSpeed { get; private set; }
        [field: SerializeField, Tooltip("Base attack stat for the character")]
        public int BaseAttack { get; private set; }
        [field: SerializeField, Tooltip("Base knockback on attack stat for the character")]
        public float BaseAttackKnockback { get; private set; }
        [field: SerializeField, Tooltip("Base defense stat for the character")]
        public int BaseDefense { get; private set; }

        [Space]
        [Header("Movement & Physics")]
        [Tooltip("Resistance to knockback effects (0 = no resistance, 1 = full resistance)")]
        [Range(0f, 1f)]
        public float KnockbackResist = 0f;
        [Tooltip("If false, the NPC will remain idle and not move")]
        public bool CanMove = true;

        [Space]
        [Header("Health & Survival")]
        [Tooltip("Duration of invincibility frames when character is hit")]
        public float IFrameDuration = 0.17f;
        [Tooltip("If true, the NPC can be knocked back")]
        public bool CanBeKnockedBack = true;
        [Tooltip("If true, character can die")]
        public bool CanDie = true;

        [Space]
        [Header("NPC Specific")]
        [Tooltip("The String ID of the NPC used for serialization not runtime lookups and networksyncing")]
        public string StringID;
        [Tooltip("Prefab for the NPC")]
        public GameObject NpcPrefab;

        [Space]
        [Header("Sounds")]
        public EventReference HurtSound;
        public EventReference DeathSound;
    }
}
