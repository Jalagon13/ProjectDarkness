using UnityEngine;

namespace ProjectDarkness
{
    public class Spell : MonoBehaviour
    {
        private Vector3 _travelDirection;
        private Vector3 _lastPosition;
        private float _distanceTravelled;
        private bool _isActive;
        private SpellData _spellData;

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

            if (_spellData == null)
            {
                Destroy(gameObject);
                return;
            }

            float frameDistance = _spellData.Speed * Time.deltaTime;
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

            if (_distanceTravelled >= _spellData.Distance)
            {
                Destroy(gameObject);
            }
        }

        public void Cast(SpellData spellData, Vector3 direction)
        {
            _spellData = spellData;
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
