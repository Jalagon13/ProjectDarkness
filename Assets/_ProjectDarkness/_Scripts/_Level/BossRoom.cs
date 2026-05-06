using UnityEngine;

namespace ProjectDarkness
{
    public class BossRoom : CombatRoom
    {
        protected override void OnRoomClear()
        {
            base.OnRoomClear();

            GameManager.Instance.CompleteGame();
        }
    }
}
