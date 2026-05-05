using UnityEngine;

namespace ProjectDarkness
{
    public class DoorCollider : MonoBehaviour
    {
        [SerializeField] private Wall _wall;
    
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                _wall.OnPlayerEnterDoor();
            }
        }
    }
}
