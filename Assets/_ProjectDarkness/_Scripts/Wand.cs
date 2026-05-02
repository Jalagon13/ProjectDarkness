using UnityEngine;

namespace ProjectDarkness
{
    public class Wand : MonoBehaviour
    {
        [SerializeField] private WandData _wandData;
        [SerializeField] private Spell _basicSpellProjectilePrefab;
        [field: SerializeField] public Transform CastPoint { get; private set; }
        
        private float _aimRayDistance = 1000f;

        private void Start()
        {
            GameInput.Instance.OnCastSpell += CastSpell;
        }

        private void OnDestroy()
        {
            GameInput.Instance.OnCastSpell -= CastSpell;
        }

        private void CastSpell()
        {
            if (_basicSpellProjectilePrefab == null || CastPoint == null)
            {
                return;
            }

            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                return;
            }

            Ray aimRay = new(mainCamera.transform.position, mainCamera.transform.forward);
            Vector3 targetPoint = aimRay.origin + (aimRay.direction * _aimRayDistance);

            if (Physics.Raycast(aimRay, out RaycastHit hit, _aimRayDistance))
            {
                targetPoint = hit.point;
            }

            Vector3 projectileDirection = (targetPoint - CastPoint.position).normalized;
            if (projectileDirection.sqrMagnitude <= Mathf.Epsilon)
            {
                projectileDirection = CastPoint.forward;
            }

            Spell spellProjectile = Instantiate(
                _basicSpellProjectilePrefab,
                CastPoint.position,
                Quaternion.LookRotation(projectileDirection));

            spellProjectile.Cast(projectileDirection);
        }
    }
}
