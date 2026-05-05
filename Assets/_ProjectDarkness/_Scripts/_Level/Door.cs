using UnityEngine;

namespace ProjectDarkness
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private GameObject _openDoor;
        [SerializeField] private GameObject _closedDoor;
    
        private DoorState _currentDoorState = DoorState.Open;
        
        public void SetDoorState(DoorState doorState)
        {
            _currentDoorState = doorState;
            
            switch(_currentDoorState)
            {
                case DoorState.Open:
                    _openDoor.SetActive(true);
                    _closedDoor.SetActive(false);
                    break;
                case DoorState.Closed:
                    _openDoor.SetActive(false);
                    _closedDoor.SetActive(true);
                    break;
            }
        }
    }
}
