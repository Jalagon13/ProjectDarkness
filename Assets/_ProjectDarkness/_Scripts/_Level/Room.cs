using System;
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

        public event Action OnNavMeshBuildStarted;
        public event Action OnNavMeshBuildCompleted;
        public event Action OnNavMeshCleared;
        
        private RoomNavMeshHandler _roomNavMeshHandler;
        public bool IsNavMeshReady => _roomNavMeshHandler != null && _roomNavMeshHandler.IsNavMeshReady;
        
        protected virtual void Awake()
        {
            _roomNavMeshHandler = GetComponent<RoomNavMeshHandler>();
            _roomNavMeshHandler.OnNavMeshBuildStarted += HandleNavMeshBuildStarted;
            _roomNavMeshHandler.OnNavMeshBuildCompleted += HandleNavMeshBuildCompleted;
            _roomNavMeshHandler.OnNavMeshCleared += HandleNavMeshCleared;
        }

        protected virtual void OnDestroy()
        {
            if (_roomNavMeshHandler == null)
            {
                return;
            }

            _roomNavMeshHandler.OnNavMeshBuildStarted -= HandleNavMeshBuildStarted;
            _roomNavMeshHandler.OnNavMeshBuildCompleted -= HandleNavMeshBuildCompleted;
            _roomNavMeshHandler.OnNavMeshCleared -= HandleNavMeshCleared;
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

        private void HandleNavMeshBuildStarted()
        {
            OnNavMeshBuildStarted?.Invoke();
        }

        private void HandleNavMeshBuildCompleted()
        {
            OnNavMeshBuildCompleted?.Invoke();
        }

        private void HandleNavMeshCleared()
        {
            OnNavMeshCleared?.Invoke();
        }
    }
}
