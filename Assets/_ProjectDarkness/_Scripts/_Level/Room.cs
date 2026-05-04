using UnityEngine;

namespace ProjectDarkness
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private WallSlot _northWallSlot;
        [SerializeField] private WallSlot _southWallSlot;
        [SerializeField] private WallSlot _eastWallSlot;
        [SerializeField] private WallSlot _westWallSlot;
    
        public void Initialize(RoomEntry roomEntry)
        {
            _northWallSlot.SetWallState(roomEntry.HasNorthDoor ? WallState.Doorway : WallState.Solid);
            _southWallSlot.SetWallState(roomEntry.HasSouthDoor ? WallState.Doorway : WallState.Solid);
            _eastWallSlot.SetWallState(roomEntry.HasEastDoor ? WallState.Doorway : WallState.Solid);
            _westWallSlot.SetWallState(roomEntry.HasWestDoor ? WallState.Doorway : WallState.Solid);
        }
    }
}