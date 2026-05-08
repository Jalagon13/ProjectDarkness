using TMPro;
using UnityEngine;

namespace ProjectDarkness
{
    public class DamagePopupUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private float _lifetime = 0.8f;
        [SerializeField] private Vector3 _initialWorldFloatVelocity = new(0f, 1.25f, 0f);
        [SerializeField, Range(0f, 1f)] private float _fadeStartNormalized = 0.55f;

        private RectTransform _rectTransform;
        private RectTransform _popupParent;
        private Canvas _canvas;
        private Camera _worldCamera;
        private Vector3 _worldPosition;
        private Vector3 _currentVelocity;
        private float _elapsedTime;
        private Color _baseColor = Color.white;

        private void Awake()
        {
            _rectTransform = (RectTransform)transform;

            if (_text == null)
            {
                _text = GetComponent<TMP_Text>();
            }

            if (_text != null)
            {
                _baseColor = _text.color;
            }
        }

        public void Initialize(int damageAmount, Vector3 worldPosition, RectTransform popupParent, Camera worldCamera)
        {
            _popupParent = popupParent;
            _canvas = popupParent != null ? popupParent.GetComponentInParent<Canvas>() : null;
            _worldCamera = worldCamera;
            _worldPosition = worldPosition;
            _currentVelocity = _initialWorldFloatVelocity;
            _elapsedTime = 0f;

            if (_text != null)
            {
                _text.text = damageAmount.ToString();
                _text.color = _baseColor;
            }

            UpdatePopupPosition();
        }

        private void Update()
        {
            _elapsedTime += Time.deltaTime;

            if (_elapsedTime >= _lifetime)
            {
                Destroy(gameObject);
                return;
            }

            float remainingLifetimeNormalized = 1f - Mathf.Clamp01(_elapsedTime / _lifetime);
            _currentVelocity = _initialWorldFloatVelocity * remainingLifetimeNormalized;
            _worldPosition += _currentVelocity * Time.deltaTime;

            UpdatePopupPosition();
            UpdateFade();
        }

        private void UpdatePopupPosition()
        {
            if (_popupParent == null || _worldCamera == null || _text == null)
            {
                return;
            }

            Vector3 viewportPoint = _worldCamera.WorldToViewportPoint(_worldPosition);
            bool isVisible =
                viewportPoint.z > 0f &&
                viewportPoint.x >= 0f && viewportPoint.x <= 1f &&
                viewportPoint.y >= 0f && viewportPoint.y <= 1f;

            _text.enabled = isVisible;
            if (!isVisible)
            {
                return;
            }

            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(_worldCamera, _worldPosition);
            Camera screenCamera = _canvas != null && _canvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? _worldCamera
                : null;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_popupParent, screenPoint, screenCamera, out Vector2 localPoint);
            _rectTransform.anchoredPosition = localPoint;
        }

        private void UpdateFade()
        {
            if (_text == null)
            {
                return;
            }

            float normalizedLifetime = Mathf.Clamp01(_elapsedTime / _lifetime);
            float alpha = 1f;

            if (normalizedLifetime >= _fadeStartNormalized)
            {
                float fadeProgress = Mathf.InverseLerp(_fadeStartNormalized, 1f, normalizedLifetime);
                alpha = 1f - fadeProgress;
            }

            Color textColor = _baseColor;
            textColor.a = alpha;
            _text.color = textColor;
        }
    }
}
