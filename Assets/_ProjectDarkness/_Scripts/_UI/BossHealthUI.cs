using System;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectDarkness
{
    public class BossHealthUI : MonoBehaviour
    {
        [SerializeField] private RectTransform _bossHealthBar;
        [SerializeField] private Image _bossHealthBarForground;
        
        private Npc _bossNpc;
        
        private void Start()
        {
            LevelManager.Instance.OnRoomSetActive += CheckForBossRoom;
            GameManager.Instance.OnGameComplete += Hide;
            
            Hide();
        }
        
        private void OnDestroy()
        {
            LevelManager.Instance.OnRoomSetActive -= CheckForBossRoom;
            GameManager.Instance.OnGameComplete -= Hide;
            
            if(_bossNpc != null)
            {
                _bossNpc.OnHealthUpdated -= UpdateBossHealthBar;
            }
        }

        private void CheckForBossRoom()
        {
            if(LevelManager.Instance.CurrentActiveRoomEntry.RoomType == RoomType.BossRoom)
            {
                BossRoom bossRoom = (BossRoom)LevelManager.Instance.SpawnedRooms[LevelManager.Instance.CurrentActiveRoomEntry.RoomCoord];

                _bossNpc = bossRoom.BossNpc;
                _bossNpc.OnHealthUpdated += UpdateBossHealthBar;
            
                Show();
            }
            else
            {
                Hide();
            }
        }

        private void UpdateBossHealthBar(object sender, EventArgs e)
        {
            _bossHealthBarForground.fillAmount = _bossNpc.CurrentHealth / _bossNpc.Data.BaseHealth;
        }

        private void Show()
        {
            _bossHealthBar.gameObject.SetActive(true);
        }
        
        private void Hide()
        {
            _bossHealthBar.gameObject.SetActive(false);
        }
    }
}
