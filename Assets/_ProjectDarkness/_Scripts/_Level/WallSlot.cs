using UnityEngine;

namespace ProjectDarkness
{
    public enum WallState
    {
        Solid,
        Doorway
    }

    public class WallSlot : MonoBehaviour
    {
        [SerializeField] private GameObject _solidWall;
        [SerializeField] private GameObject _doorwayWall;
        
        private WallState _wallState;

        public void SetWallState(WallState wallState)
        {
            _wallState = wallState;
            
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
    }
}
