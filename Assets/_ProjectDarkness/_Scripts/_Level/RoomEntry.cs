using UnityEngine;

namespace ProjectDarkness
{
    public class RoomEntry
    {
        public DoorConnection NorthDoorConnection = new();
        public DoorConnection SouthDoorConnection = new();
        public DoorConnection EastDoorConnection = new();
        public DoorConnection WestDoorConnection = new();

        private readonly Vector2Int _roomCoord;
        public Vector2Int RoomCoord => _roomCoord;
        public RoomType RoomType { get; private set; }
        
        public RoomEntry(Vector2Int roomCoord)
        {
            _roomCoord = roomCoord;
        }
        
        public void SetRoomType(RoomType roomType)
        {
            RoomType = roomType;
        }
    }
}
