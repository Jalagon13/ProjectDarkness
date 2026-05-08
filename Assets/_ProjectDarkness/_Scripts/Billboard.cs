using UnityEngine;

namespace ProjectDarkness
{
    public class Billboard : MonoBehaviour
    {
        [SerializeField]
        private bool _faceCameraAtAnyAngle;

        private Camera _mainCamera;
    
        private void Start()
        {
            _mainCamera = Camera.main;
        }
    
        private void LateUpdate()
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
                if (_mainCamera == null)
                {
                    return;
                }
            }

            Vector3 cameraPosition = _mainCamera.transform.position;

            if (!_faceCameraAtAnyAngle)
            {
                cameraPosition.y = transform.position.y;
            }
            
            transform.LookAt(cameraPosition);
            
            transform.Rotate(0f, 180f, 0f);
        }
    }
}
