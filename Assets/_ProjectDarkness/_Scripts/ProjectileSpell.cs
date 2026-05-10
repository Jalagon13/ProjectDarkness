using System;
using UnityEngine;

namespace ProjectDarkness
{
    public enum ProjectileDestroyReason
    {
        Unknown = 0,
        NpcHit = 1,
        LifetimeExpired = 2,
        WallHit = 3,
        BounceLimitReached = 4
    }

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    public class ProjectileSpell : MonoBehaviour
    {
        public event Action<ProjectileSpell, Collision> OnProjectileBounce;
        public event Action<ProjectileSpell, Npc> OnProjectileHitNpc;
        public event Action<ProjectileSpell> OnProjectileExpired;
        public event Action<ProjectileSpell, ProjectileDestroyReason> OnProjectileDestroyed;

        [SerializeField] 
        private GameObject _spellModifierHolder;

        private Rigidbody _rigidbody;
        private Vector3 _travelDirection;
        private bool _isActive;
        private bool _isEnding;
        private bool _destroyedEventRaised;
        private int _remainingBounces;
        private int _lastResolvedCollisionFrame = -1;
        private Timer _lifetimeTimer;
        private ProjectileDestroyReason _destroyReason = ProjectileDestroyReason.Unknown;
        
        private ProjectileSpellRuntimeData _runtimeData;
        public ProjectileSpellRuntimeData RuntimeData => _runtimeData;


        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.useGravity = false;
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        private void OnDestroy()
        {
            CleanupLifetimeTimer();

            if (_destroyedEventRaised)
            {
                return;
            }

            RaiseDestroyedEvent(_destroyReason);
        }

        private void Update()
        {
            if (!_isActive)
            {
                return;
            }

            _lifetimeTimer?.Tick(Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isActive || _isEnding)
            {
                return;
            }

            if (!TryGetNpc(other, out Npc npc))
            {
                return;
            }

            Vector3 hitPoint = other.ClosestPoint(transform.position);
            npc.TakeDamage(_runtimeData.Damage, hitPoint);
            OnProjectileHitNpc?.Invoke(this, npc);
            EndProjectile(ProjectileDestroyReason.NpcHit);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!_isActive || _isEnding)
            {
                return;
            }

            if (TryGetNpc(collision.collider, out Npc npc))
            {
                Vector3 hitPoint = collision.contactCount > 0
                    ? collision.GetContact(0).point
                    : collision.collider.ClosestPoint(transform.position);
                npc.TakeDamage(_runtimeData.Damage, hitPoint);
                OnProjectileHitNpc?.Invoke(this, npc);
                EndProjectile(ProjectileDestroyReason.NpcHit);
                return;
            }

            ResolveBounceOrDestroy(collision);
        }

        public void Initialize(CastContext castContext)
        {
            _runtimeData = new ProjectileSpellRuntimeData(castContext);

            foreach (ModifierSpellData modifierData in castContext.Modifiers)
            {
                ModifierSpell modifierSpell = Instantiate(modifierData.ModifierSpellPrefab, _spellModifierHolder.transform);
                modifierSpell.ModifySpell(this, modifierData);
                
                Debug.Log($"Added Modifier Spell: {modifierData.SpellName} for Projectile Spell: {castContext.ProjectileSpell.SpellName}");
            }

            _remainingBounces = _runtimeData.BounceCount;
        }

        public void Cast(Vector3 direction)
        {
            _travelDirection = direction.normalized;

            if (_travelDirection.sqrMagnitude <= Mathf.Epsilon)
            {
                _travelDirection = transform.forward;
            }

            transform.forward = _travelDirection;
            
            _lifetimeTimer = new Timer(_runtimeData.Lifetime);
            _lifetimeTimer.OnTimerEnd += OnLifetimeTimerEnd;

            _isActive = true;
            ApplyVelocity(_travelDirection);
        }

        private void OnLifetimeTimerEnd(object sender, EventArgs e)
        {
            OnProjectileExpired?.Invoke(this);
            EndProjectile(ProjectileDestroyReason.LifetimeExpired);
        }

        private void ResolveBounceOrDestroy(Collision collision)
        {
            if (_lastResolvedCollisionFrame == Time.frameCount)
            {
                return;
            }

            _lastResolvedCollisionFrame = Time.frameCount;

            if (_remainingBounces <= 0)
            {
                EndProjectile(ProjectileDestroyReason.BounceLimitReached);
                return;
            }

            if (collision.contactCount <= 0)
            {
                EndProjectile(ProjectileDestroyReason.WallHit);
                return;
            }

            ContactPoint contact = collision.GetContact(0);
            Vector3 reflectedDirection = Vector3.Reflect(_travelDirection, contact.normal).normalized;
            if (reflectedDirection.sqrMagnitude <= Mathf.Epsilon)
            {
                EndProjectile(ProjectileDestroyReason.WallHit);
                return;
            }

            _remainingBounces--;
            _lifetimeTimer.AddTime(1f);
            _travelDirection = reflectedDirection;
            transform.forward = _travelDirection;
            ApplyVelocity(_travelDirection);
            OnProjectileBounce?.Invoke(this, collision);
        }

        private void ApplyVelocity(Vector3 direction)
        {
            _rigidbody.linearVelocity = direction * _runtimeData.Speed;
        }

        private bool TryGetNpc(Collider other, out Npc npc)
        {
            npc = other.GetComponentInParent<Npc>();
            return npc != null;
        }

        private void EndProjectile(ProjectileDestroyReason reason)
        {
            if (_isEnding)
            {
                return;
            }

            _isEnding = true;
            _isActive = false;
            _destroyReason = reason;

            CleanupLifetimeTimer();

            if (_rigidbody != null)
            {
                _rigidbody.linearVelocity = Vector3.zero;
            }

            RaiseDestroyedEvent(reason);
            Destroy(gameObject);
        }

        private void CleanupLifetimeTimer()
        {
            if (_lifetimeTimer == null)
            {
                return;
            }

            _lifetimeTimer.OnTimerEnd -= OnLifetimeTimerEnd;
            _lifetimeTimer = null;
        }

        private void RaiseDestroyedEvent(ProjectileDestroyReason reason)
        {
            if (_destroyedEventRaised)
            {
                return;
            }

            _destroyedEventRaised = true;
            OnProjectileDestroyed?.Invoke(this, reason);
        }
    }
}
