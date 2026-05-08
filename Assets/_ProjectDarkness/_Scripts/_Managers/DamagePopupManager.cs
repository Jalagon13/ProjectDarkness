using UnityEngine;

namespace ProjectDarkness
{
    public class DamagePopupManager : MonoBehaviour
    {
        public static DamagePopupManager Instance { get; private set; }

        [SerializeField] private DamagePopupUI _damagePopupPrefab;
        [SerializeField] private RectTransform _popupParent;
        [SerializeField] private float _spawnPositionRandomOffsetRadius = 0.25f;

        private Camera _worldCamera;

        private void Awake()
        {
            Instance = this;

            if (_popupParent == null)
            {
                Debug.LogWarning("DamagePopupManager is missing a popup parent reference.");
            }
        }

        public void ShowDamagePopup(int damageAmount, Vector3 worldPosition)
        {
            if (_damagePopupPrefab == null || _popupParent == null)
            {
                return;
            }

            if (_worldCamera == null)
            {
                _worldCamera = Camera.main;
            }

            Vector3 spawnPosition = worldPosition + Random.insideUnitSphere * _spawnPositionRandomOffsetRadius;
            DamagePopupUI popupInstance = Instantiate(_damagePopupPrefab, _popupParent);
            popupInstance.transform.SetAsLastSibling();
            popupInstance.Initialize(damageAmount, spawnPosition, _popupParent, _worldCamera);
        }
    }
}
