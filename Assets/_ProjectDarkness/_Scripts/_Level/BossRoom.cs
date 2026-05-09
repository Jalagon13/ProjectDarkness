using UnityEngine;

namespace ProjectDarkness
{
    public class BossRoom : CombatRoom
    {
        [Header("Boss Room Settings")]
        [SerializeField] private Npc _bossNpc;
        public Npc BossNpc => _bossNpc;

        protected override void Awake()
        {
            base.Awake();
        
            if(_bossNpc == null)
            {
                Debug.LogError($"Boss should not be null for a bossroom");
            }
        }

        protected override void OnFirstVisit()
        {
            base.OnFirstVisit();

            // Enable boss health
            

        }


        protected override void OnRoomClear()
        {
            base.OnRoomClear();
            
            // Disable boss health

            GameManager.Instance.CompleteGame();
        }
    }
}
