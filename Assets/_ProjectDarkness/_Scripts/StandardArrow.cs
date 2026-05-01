using UnityEngine;

namespace ProjectDarkness
{
    [RequireComponent(typeof(Rigidbody))]
    public class StandardArrow : MonoBehaviour
    {
        [field: SerializeField] private Transform ArrowRearPoint;
        [SerializeField] [Min(0f)] private float _launchForce = 20f;
        [SerializeField] [Min(0f)] private float _rotationLerpSpeed = 12f;
        [SerializeField] [Min(0f)] private float _launchOriginForwardOffset = 0.2f;

        public Transform RearPoint => ArrowRearPoint;

        private Rigidbody _rigidbody;
        private Vector3 _localTravelDirection;
        private bool _isLaunched;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();

            if (ArrowRearPoint != null)
            {
                _localTravelDirection = (-ArrowRearPoint.localPosition).normalized;
            }
            else
            {
                _localTravelDirection = Vector3.forward;
            }
        }

        private void Start()
        {
            SetLoadedState();
        }

        private void Update()
        {
            if (!_isLaunched)
            {
                return;
            }

            Vector3 velocity = _rigidbody.linearVelocity;
            if (velocity.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            Vector3 currentTravelDirection = transform.TransformDirection(_localTravelDirection);
            Quaternion targetRotation = Quaternion.FromToRotation(currentTravelDirection, velocity.normalized) * transform.rotation;
            float lerpStep = 1f - Mathf.Exp(-_rotationLerpSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lerpStep);
        }

        public void SetLoadedState()
        {
            _isLaunched = false;
            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }

        public void Launch(float chargePercent, Transform launchOrigin = null)
        {
            Vector3 launchDirection = PrepareLaunchFromOrigin(launchOrigin);

            transform.SetParent(null, true);
            SetLayerRecursively(gameObject, 0);

            _isLaunched = true;
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.AddForce(launchDirection * (_launchForce * Mathf.Clamp01(chargePercent)), ForceMode.Impulse);
        }

        private Vector3 PrepareLaunchFromOrigin(Transform launchOrigin)
        {
            if (launchOrigin == null)
            {
                Camera mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    launchOrigin = mainCamera.transform;
                }
            }

            if (launchOrigin != null)
            {
                Vector3 targetDirection = launchOrigin.forward.normalized;
                Vector3 currentDirection = GetLaunchDirection();

                if (currentDirection.sqrMagnitude > 0.0001f && targetDirection.sqrMagnitude > 0.0001f)
                {
                    Quaternion rotationOffset = Quaternion.FromToRotation(currentDirection, targetDirection);
                    transform.rotation = rotationOffset * transform.rotation;
                }

                if (ArrowRearPoint != null)
                {
                    Vector3 launchPosition = launchOrigin.position + (targetDirection * _launchOriginForwardOffset);
                    transform.position += launchPosition - ArrowRearPoint.position;
                }

                return targetDirection;
            }

            Vector3 fallbackDirection = GetLaunchDirection();
            if (fallbackDirection.sqrMagnitude <= 0.0001f)
            {
                return transform.forward;
            }

            return fallbackDirection;
        }

        private Vector3 GetLaunchDirection()
        {
            if (ArrowRearPoint == null)
            {
                return transform.forward;
            }

            return (transform.position - ArrowRearPoint.position).normalized;
        }

        private static void SetLayerRecursively(GameObject target, int layer)
        {
            target.layer = layer;

            foreach (Transform child in target.transform)
            {
                SetLayerRecursively(child.gameObject, layer);
            }
        }
    }
}
