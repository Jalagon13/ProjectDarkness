using UnityEngine;

namespace ProjectDarkness
{
    public class ChaserNpc : Npc
    {
        [Header("Chaser AI")]
        [SerializeField] private float _repathInterval = 0.1f;
        [SerializeField] private float _turnSpeed = 12f;
        [SerializeField] private float _contactDamageCooldown = 0.5f;

        private float _repathTimer;
        private float _contactDamageTimer;

        private void Update()
        {
            if (!IsAiEnabled || Player.Instance == null || HealthManager.Instance == null || HealthManager.Instance.IsDead)
            {
                return;
            }

            _repathTimer -= Time.deltaTime;
            _contactDamageTimer -= Time.deltaTime;

            if (_repathTimer <= 0f)
            {
                _repathTimer = _repathInterval;
                NavMeshAgent.SetDestination(Player.Instance.transform.position);
            }

            Vector3 desiredVelocity = NavMeshAgent.desiredVelocity;
            if (desiredVelocity.sqrMagnitude <= 0.001f)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(desiredVelocity.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _turnSpeed * Time.deltaTime);
        }

        private void OnCollisionStay(Collision collision)
        {
            if (!IsAiEnabled || _contactDamageTimer > 0f)
            {
                return;
            }

            if (!TryGetPlayer(collision.collider, out _))
            {
                return;
            }

            HealthManager.Instance.RemoveHealth(Data.BaseAttack);
            _contactDamageTimer = _contactDamageCooldown;
        }

        private void OnTriggerStay(Collider other)
        {
            if (!IsAiEnabled || _contactDamageTimer > 0f)
            {
                return;
            }

            if (!TryGetPlayer(other, out _))
            {
                return;
            }

            HealthManager.Instance.RemoveHealth(Data.BaseAttack);
            _contactDamageTimer = _contactDamageCooldown;
        }

        protected override void OnAiEnabled()
        {
            base.OnAiEnabled();

            if (!NavMeshAgent.enabled)
            {
                return;
            }

            NavMeshAgent.SetDestination(Player.Instance.transform.position);
            _repathTimer = 0f;
            _contactDamageTimer = 0f;
        }

        protected override void OnAiDisabled()
        {
            base.OnAiDisabled();
            _repathTimer = 0f;
            _contactDamageTimer = 0f;
        }

        private bool TryGetPlayer(Collider other, out Player player)
        {
            player = other.GetComponentInParent<Player>();
            return player != null;
        }
    }
}
