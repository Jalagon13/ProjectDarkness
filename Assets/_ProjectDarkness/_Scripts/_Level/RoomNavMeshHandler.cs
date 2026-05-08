using Unity.AI.Navigation;
using UnityEngine;

namespace ProjectDarkness
{
    [RequireComponent(typeof(NavMeshSurface))]
    public class RoomNavMeshHandler : MonoBehaviour
    {
        private NavMeshSurface _navMeshSurface;
        
        private void Awake()
        {
            _navMeshSurface = GetComponent<NavMeshSurface>();
        }
        
        public void BuildNavMesh()
        {
            _navMeshSurface.BuildNavMesh();
        }
        
        public void UpdateNavMesh()
        {
            _navMeshSurface.UpdateNavMesh(_navMeshSurface.navMeshData);
        }
        
        public void ClearNavMesh()
        {
            _navMeshSurface.navMeshData = null;
        }
    }
}
