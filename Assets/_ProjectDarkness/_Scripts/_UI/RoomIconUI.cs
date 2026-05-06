using UnityEngine;
using UnityEngine.UI;

namespace ProjectDarkness
{
    public class RoomIconUI : MonoBehaviour
    {
        [SerializeField] private GameObject _visitedRoomIcon;
        [SerializeField] private GameObject _unvisitedRoomIcon;

        [Header("Room Info")]
        [SerializeField] private Image _roomIconImage;
        [SerializeField] private Sprite _bossRoomIconSprite;

        private RoomIconState _roomIconState = RoomIconState.UnDiscovered;
        public RoomIconState RoomIconState => _roomIconState;

        private void Awake()
        {
            SetRoomIconState(RoomIconState.UnDiscovered);
        }

        public void Initialize(Vector2Int roomCoord)
        {
            RoomEntry roomEntry = LevelManager.Instance.FloorPlan[roomCoord];

            if (roomEntry.RoomType == RoomType.BossRoom)
            {
                _roomIconImage.sprite = _bossRoomIconSprite;
                _roomIconImage.enabled = true;
            }
            else
            {
                _roomIconImage.sprite = null;
                _roomIconImage.enabled = false;
            }
        }

        public void SetRoomIconState(RoomIconState roomIconState)
        {
            _roomIconState = roomIconState;
            
            // Reset alpha when state changes
            Color c = _roomIconImage.color;
            c.a = 1.0f;
            _roomIconImage.color = c;

            switch (_roomIconState)
            {
                case RoomIconState.Visited:
                    _visitedRoomIcon.SetActive(true);
                    _unvisitedRoomIcon.SetActive(false);
                    _roomIconImage.gameObject.SetActive(true);
                    break;
                case RoomIconState.Unvisited:
                    _visitedRoomIcon.SetActive(false);
                    _unvisitedRoomIcon.SetActive(true);
                    _roomIconImage.gameObject.SetActive(true);
                    break;
                case RoomIconState.UnDiscovered:
                    _visitedRoomIcon.SetActive(false);
                    _unvisitedRoomIcon.SetActive(false);
                    _roomIconImage.gameObject.SetActive(false);
                    break;
            }
        }

        public void SetRoomIconDimmed(bool isDimmed)
        {
            if (_roomIconState == RoomIconState.UnDiscovered) return;
            
            Color c = _roomIconImage.color;
            c.a = isDimmed ? 0.15f : 1.0f;
            _roomIconImage.color = c;
        }
    }

    public enum RoomIconState
    {
        Visited,
        Unvisited,
        UnDiscovered
    }
}
