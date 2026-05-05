using UnityEngine;

namespace ProjectDarkness
{
    public enum WallState
    {
        Solid,
        Doorway
    }

    public class Wall : MonoBehaviour
    {
        [SerializeField] private GameObject _solidWall;
        [SerializeField] private GameObject _doorwayWall;
        [SerializeField] private Transform _onEnterPlacementPoint;
        public Transform OnEnterPlacementPoint => _onEnterPlacementPoint;
        
        private WallState _wallState;
        private RoomEntry _roomEntry;
        private CardinalDirection _direction;
        

        public void Initialize(WallState wallState, RoomEntry roomEntry, CardinalDirection direction)
        {
            _wallState = wallState;
            _roomEntry = roomEntry;
            _direction = direction;

            SetupWall();
        }
        
        private void SetupWall()
        {
            switch (_wallState)
            {
                case WallState.Solid:
                    _solidWall.SetActive(true);
                    _doorwayWall.SetActive(false);
                    break;
                case WallState.Doorway:
                    _solidWall.SetActive(false);
                    _doorwayWall.SetActive(true);
                    break;
            }
        }
        
        public void OnPlayerEnterDoor()
        {
            switch(_direction)
            {
                case CardinalDirection.North:
                    LevelManager.Instance.TransitionRoom(_roomEntry.NorthDoorConnection.TargetRoomEntry, _direction);
                    break;
                case CardinalDirection.South:
                    LevelManager.Instance.TransitionRoom(_roomEntry.SouthDoorConnection.TargetRoomEntry, _direction);
                    break;
                case CardinalDirection.East:
                    LevelManager.Instance.TransitionRoom(_roomEntry.EastDoorConnection.TargetRoomEntry, _direction);
                    break;
                case CardinalDirection.West:
                    LevelManager.Instance.TransitionRoom(_roomEntry.WestDoorConnection.TargetRoomEntry, _direction);
                    break;
            }
        }
    }
}
