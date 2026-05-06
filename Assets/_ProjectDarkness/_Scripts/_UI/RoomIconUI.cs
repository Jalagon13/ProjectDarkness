using UnityEngine;

namespace ProjectDarkness
{
    public class RoomIconUI : MonoBehaviour
    {
        [SerializeField] private GameObject _visitedRoomIcon;
        [SerializeField] private GameObject _unvisitedRoomIcon;
    
        private RoomIconState _roomIconState = RoomIconState.UnDiscovered;
        public RoomIconState RoomIconState => _roomIconState;
        
        private void Awake()
        {
            SetRoomIconState(RoomIconState.UnDiscovered);
        }
        
        public void SetRoomIconState(RoomIconState roomIconState)
        {
            _roomIconState = roomIconState;
            
            switch(_roomIconState)
            {
                case RoomIconState.Visited:
                    _visitedRoomIcon.SetActive(true);
                    _unvisitedRoomIcon.SetActive(false);
                    break;
                case RoomIconState.Unvisited:
                    _visitedRoomIcon.SetActive(false);
                    _unvisitedRoomIcon.SetActive(true);
                    break;
                case RoomIconState.UnDiscovered:
                    _visitedRoomIcon.SetActive(false);
                    _unvisitedRoomIcon.SetActive(false);
                    break;
            }
        }
    }
    
    public enum RoomIconState
    {
        Visited,
        Unvisited,
        UnDiscovered
    }
}
