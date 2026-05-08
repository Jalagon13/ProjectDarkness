using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

namespace ProjectDarkness
{
    [RequireComponent(typeof(RoomNavMeshHandler))]
    public class Room : MonoBehaviour
    {
        [field: SerializeField] public Wall NorthWall { get; private set; }
        [field: SerializeField] public Wall SouthWall { get; private set; }
        [field: SerializeField] public Wall EastWall { get; private set; }
        [field: SerializeField] public Wall WestWall { get; private set; }

        [HideInInspector] public bool HasBeenVisited = false;
        
        private RoomNavMeshHandler _roomNavMeshHandler;
        
        private void Awake()
        {
            _roomNavMeshHandler = GetComponent<RoomNavMeshHandler>();
        }

        public void Initialize(RoomEntry roomEntry)
        {
            NorthWall.Initialize(roomEntry.NorthDoorConnection.IsEnabled ? WallState.Doorway : WallState.Solid, roomEntry, CardinalDirection.North);
            SouthWall.Initialize(roomEntry.SouthDoorConnection.IsEnabled ? WallState.Doorway : WallState.Solid, roomEntry, CardinalDirection.South);
            EastWall.Initialize(roomEntry.EastDoorConnection.IsEnabled ? WallState.Doorway : WallState.Solid, roomEntry, CardinalDirection.East);
            WestWall.Initialize(roomEntry.WestDoorConnection.IsEnabled ? WallState.Doorway : WallState.Solid, roomEntry, CardinalDirection.West);
        }
        
        public virtual void OnRoomEnter()
        {
            if (!HasBeenVisited)
            {
                HasBeenVisited = true;

                OnFirstVisit();
            }

            Debug.Log($"Entering {gameObject.name}");
            _roomNavMeshHandler.BuildNavMesh();
        }
        
        public virtual void OnRoomExit()
        {
            Debug.Log($"Exiting {gameObject.name}");
            _roomNavMeshHandler.ClearNavMesh();
        }
        
        protected virtual void OnFirstVisit()
        {
            Debug.Log($"First visit to {gameObject.name}");
        }
    }
}