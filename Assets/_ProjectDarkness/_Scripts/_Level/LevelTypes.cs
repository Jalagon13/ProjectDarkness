

namespace ProjectDarkness
{
    public enum WallState
    {
        Solid,
        Doorway
    }
    
    public enum DoorState
    {
        Open,
        Closed
    }

    public enum CardinalDirection
    {
        North,
        South,
        East,
        West
    }

    public enum RoomType
    {
        StartingRoom,
        CombatRoom,
        BossRoom,
        Shoproom
    }
    
    public enum CombatRoomState
    {
        Cleared,
        HasEnemies
    }
    
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

            if (TargetRoomEntry == null)
            {
                IsEnabled = false;
            }
        }
    }
    
    
}