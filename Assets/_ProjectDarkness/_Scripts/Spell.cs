using UnityEngine;

namespace ProjectDarkness
{
    public class Spell : MonoBehaviour
    {
        [SerializeField] private float _speed = 10f;
        [SerializeField] private float _distance = 10f;
        [SerializeField] private int _manaReq = 10;

        private Vector3 _travelDirection;
        private Vector3 _lastPosition;
        private float _distanceTravelled;
        private bool _isActive;

        private void Awake()
        {
            _lastPosition = transform.position;
        }

        private void Update()
        {
            if (!_isActive)
            {
                return;
            }

            float frameDistance = _speed * Time.deltaTime;
            if (frameDistance <= 0f)
            {
                return;
            }

            if (Physics.Raycast(_lastPosition, _travelDirection, frameDistance))
            {
                Destroy(gameObject);
                return;
            }

            transform.position += _travelDirection * frameDistance;
            _distanceTravelled += frameDistance;
            _lastPosition = transform.position;

            if (_distanceTravelled >= _distance)
            {
                Destroy(gameObject);
            }
        }

        public void Cast(Vector3 direction)
        {
            _travelDirection = direction.normalized;

            if (_travelDirection.sqrMagnitude <= Mathf.Epsilon)
            {
                _travelDirection = transform.forward;
            }

            transform.forward = _travelDirection;
            _distanceTravelled = 0f;
            _lastPosition = transform.position;
            _isActive = true;
        }
    }
}
