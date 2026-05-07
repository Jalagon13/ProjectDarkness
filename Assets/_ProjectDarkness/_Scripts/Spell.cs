using System;
using UnityEngine;

namespace ProjectDarkness
{
    public class Spell : MonoBehaviour
    {
        [field: SerializeField] public SpellData Data { get; private set; }

        private Vector3 _travelDirection;
        private Vector3 _lastPosition;
        private bool _isActive;
        private Timer _lifetimeTimer;

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

            _lifetimeTimer?.Tick(Time.deltaTime);

            float frameDistance = Data.Speed * Time.deltaTime;
            if (frameDistance <= 0f)
            {
                return;
            }

            if (Physics.Raycast(_lastPosition, _travelDirection, out RaycastHit hit, frameDistance) && hit.collider.gameObject.layer == 3)
            {
                Destroy(gameObject);
                return;
            }

            transform.position += _travelDirection * frameDistance;
            _lastPosition = transform.position;
        }

        public void Cast(Vector3 direction)
        {
            _travelDirection = direction.normalized;

            if (_travelDirection.sqrMagnitude <= Mathf.Epsilon)
            {
                _travelDirection = transform.forward;
            }

            transform.forward = _travelDirection;
            
            _lifetimeTimer = new Timer(Data.Lifetime);
            _lifetimeTimer.OnTimerEnd += OnLifetimeTimerEnd;

            _lastPosition = transform.position;
            _isActive = true;
        }

        private void OnLifetimeTimerEnd(object sender, EventArgs e)
        {
            _lifetimeTimer.OnTimerEnd -= OnLifetimeTimerEnd;
            
            Destroy(gameObject);
        }
    }
}
