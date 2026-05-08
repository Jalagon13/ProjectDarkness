using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectDarkness
{
    public class ProjectileSpell : MonoBehaviour
    {
        [SerializeField] 
        private GameObject _spellModifierHolder;

        private Vector3 _travelDirection;
        private Vector3 _lastPosition;
        private bool _isActive;
        private Timer _lifetimeTimer;
        private ProjectileSpellRuntimeData _runtimeData;
        public ProjectileSpellRuntimeData RuntimeData => _runtimeData;


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

            float frameDistance = _runtimeData.Speed * Time.deltaTime;
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

        public void Initialize(CastContext castContext)
        {
            _runtimeData = new ProjectileSpellRuntimeData(castContext);

            foreach (ModifierSpellData modifierData in castContext.Modifiers)
            {
                Instantiate(modifierData.ModifierSpellPrefab, _spellModifierHolder.transform);
                Debug.Log($"Added Modifier Spell: {modifierData.SpellName} for Projectile Spell: {castContext.ProjectileSpell.SpellName}");
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
            
            _lifetimeTimer = new Timer(_runtimeData.Lifetime);
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
