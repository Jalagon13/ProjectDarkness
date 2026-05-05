using UnityEngine;

namespace ProjectDarkness
{
    public class DoorConnection
    {
        public bool IsEnabled { get; private set; }
        public RoomEntry TargetRoomEntry { get; private set; }
        
        public DoorConnection()
        {
            IsEnabled = false;
            TargetRoomEntry = null;
        }
        
        public void UpdateData(bool isEnabled, RoomEntry targetRoom)
        {
            IsEnabled = isEnabled;
            TargetRoomEntry = targetRoom;
            
            if(TargetRoomEntry == null)
            {
                IsEnabled = false;
            }
        }
    }

    public class RoomEntry
    {
        public DoorConnection NorthDoorConnection = new();
        public DoorConnection SouthDoorConnection = new();
        public DoorConnection EastDoorConnection = new();
        public DoorConnection WestDoorConnection = new();

        private readonly Vector2Int _roomCoord;
        public Vector2Int RoomCoord => _roomCoord;
        
        public RoomEntry(Vector2Int roomCoord)
        {
            _roomCoord = roomCoord;
        }
        
        
    }
}
