using UnityEngine;

namespace ProjectDarkness
{
    public class BowHolder : MonoBehaviour
    {
        [SerializeField] private Transform _bowIdleTf;
        [SerializeField] private Transform _bowChargingTf;
        [SerializeField] [Min(0.01f)] private float _lerpSpeed = 10f;

        private void Start()
        {
            if (_bowIdleTf == null)
            {
                return;
            }

            transform.localPosition = _bowIdleTf.localPosition;
            transform.localRotation = _bowIdleTf.localRotation;
            transform.localScale = _bowIdleTf.localScale;
        }

        private void Update()
        {
            Transform targetTransform = GetTargetTransform();
            if (targetTransform == null)
            {
                return;
            }

            float lerpStep = 1f - Mathf.Exp(-_lerpSpeed * Time.deltaTime);

            transform.localPosition = Vector3.Lerp(transform.localPosition, targetTransform.localPosition, lerpStep);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetTransform.localRotation, lerpStep);
            transform.localScale = Vector3.Lerp(transform.localScale, targetTransform.localScale, lerpStep);
        }

        private Transform GetTargetTransform()
        {
            if (BowManager.Instance != null && BowManager.Instance.IsCharging)
            {
                return _bowChargingTf != null ? _bowChargingTf : _bowIdleTf;
            }

            return _bowIdleTf;
        }
    }
}
