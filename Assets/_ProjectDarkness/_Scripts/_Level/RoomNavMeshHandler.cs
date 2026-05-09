using System;
using Unity.AI.Navigation;
using UnityEngine;

namespace ProjectDarkness
{
    [RequireComponent(typeof(NavMeshSurface))]
    public class RoomNavMeshHandler : MonoBehaviour
    {
        public event Action OnNavMeshBuildStarted;
        public event Action OnNavMeshBuildCompleted;
        public event Action OnNavMeshCleared;

        private NavMeshSurface _navMeshSurface;
        public bool IsNavMeshReady { get; private set; }
        
        private void Awake()
        {
            _navMeshSurface = GetComponent<NavMeshSurface>();
        }
        
        public void BuildNavMesh()
        {
            IsNavMeshReady = false;
            OnNavMeshBuildStarted?.Invoke();
            _navMeshSurface.BuildNavMesh();
            IsNavMeshReady = true;
            OnNavMeshBuildCompleted?.Invoke();
        }
        
        public void UpdateNavMesh()
        {
            IsNavMeshReady = false;
            OnNavMeshBuildStarted?.Invoke();
            _navMeshSurface.UpdateNavMesh(_navMeshSurface.navMeshData);
            IsNavMeshReady = true;
            OnNavMeshBuildCompleted?.Invoke();
        }
        
        public void ClearNavMesh()
        {
            IsNavMeshReady = false;
            _navMeshSurface.RemoveData();
            _navMeshSurface.navMeshData = null;
            OnNavMeshCleared?.Invoke();
        }
    }
}
