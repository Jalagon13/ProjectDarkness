using System.Collections.Generic;
using UnityEngine;

namespace ProjectDarkness
{
    public class LightningChainModifierSpell : ModifierSpell
    {
        [SerializeField] private float _detectionRaidus = 5f;
        [SerializeField] private int _detectionMaxAmount = 10;
        [SerializeField] private float _linkRadius = 0.35f;
        [SerializeField] private float _damageInterval = 0.2f;
        [SerializeField] private int _damageMaxTargets = 16;
        [SerializeField] private float _lineWidth = 0.15f;
        [SerializeField] private Material _lineMaterial;
        [SerializeField] private LayerMask _damageLayers = ~0;

        private static readonly HashSet<LightningChainModifierSpell> ActiveModifiers = new();

        private readonly Dictionary<int, ActiveLink> _activeLinks = new();
        private readonly HashSet<int> _hitNpcIds = new();

        private ProjectileSpell _projectileSpell;
        private Collider[] _damageResults;

        public override void ModifySpell(ProjectileSpell spell, ModifierSpellData modifierData)
        {
            _projectileSpell = spell;
            _damageResults = new Collider[_damageMaxTargets];
        }

        private void OnEnable()
        {
            ActiveModifiers.Add(this);
        }

        private void OnDisable()
        {
            ActiveModifiers.Remove(this);
            ClearLinks();
        }

        private void Update()
        {
            if (_projectileSpell == null)
            {
                return;
            }

            CleanupInvalidLinks();
            RefreshLinks();
            UpdateLinks();
        }

        private void CleanupInvalidLinks()
        {
            if (_activeLinks.Count == 0)
            {
                return;
            }

            _keysToRemove.Clear();

            float detectionRadiusSqr = _detectionRaidus * _detectionRaidus;
            Vector3 currentPosition = GetSpellPosition();

            foreach (KeyValuePair<int, ActiveLink> pair in _activeLinks)
            {
                ActiveLink link = pair.Value;
                if (link.Other == null || link.Other._projectileSpell == null)
                {
                    _keysToRemove.Add(pair.Key);
                    continue;
                }

                float sqrDistance = (currentPosition - link.Other.GetSpellPosition()).sqrMagnitude;
                if (sqrDistance > detectionRadiusSqr)
                {
                    _keysToRemove.Add(pair.Key);
                }
            }

            for (int i = 0; i < _keysToRemove.Count; i++)
            {
                int key = _keysToRemove[i];
                if (_activeLinks.TryGetValue(key, out ActiveLink link))
                {
                    DestroyLink(link);
                    _activeLinks.Remove(key);
                }
            }
        }

        private void RefreshLinks()
        {
            int linksCreatedThisFrame = 0;
            float detectionRadiusSqr = _detectionRaidus * _detectionRaidus;
            Vector3 currentPosition = GetSpellPosition();

            foreach (LightningChainModifierSpell other in ActiveModifiers)
            {
                if (other == null || other == this || other._projectileSpell == null)
                {
                    continue;
                }

                if (!ShouldOwnLink(this, other))
                {
                    continue;
                }

                float sqrDistance = (currentPosition - other.GetSpellPosition()).sqrMagnitude;
                if (sqrDistance > detectionRadiusSqr)
                {
                    RemoveLink(other);
                    continue;
                }

                if (_activeLinks.ContainsKey(other.GetInstanceID()))
                {
                    continue;
                }

                if (linksCreatedThisFrame >= _detectionMaxAmount)
                {
                    break;
                }

                CreateLink(other);
                linksCreatedThisFrame++;
            }
        }

        private void UpdateLinks()
        {
            foreach (ActiveLink link in _activeLinks.Values)
            {
                if (link.Other == null)
                {
                    continue;
                }

                Vector3 start = GetSpellPosition();
                Vector3 end = link.Other.GetSpellPosition();

                link.Renderer.SetPosition(0, start);
                link.Renderer.SetPosition(1, end);

                link.DamageTimer -= Time.deltaTime;
                if (link.DamageTimer > 0f)
                {
                    continue;
                }

                link.DamageTimer = _damageInterval;
                DamageAlongLink(start, end);
            }
        }

        private void DamageAlongLink(Vector3 start, Vector3 end)
        {
            _hitNpcIds.Clear();

            int hitCount = Physics.OverlapCapsuleNonAlloc(
                start,
                end,
                _linkRadius,
                _damageResults,
                _damageLayers,
                QueryTriggerInteraction.Collide);

            for (int i = 0; i < hitCount; i++)
            {
                Collider hit = _damageResults[i];
                if (hit == null)
                {
                    continue;
                }

                Npc npc = hit.GetComponentInParent<Npc>();
                if (npc == null)
                {
                    continue;
                }

                int npcId = npc.GetInstanceID();
                if (!_hitNpcIds.Add(npcId))
                {
                    continue;
                }

                Vector3 hitPoint = hit.ClosestPoint((start + end) * 0.5f);
                npc.TakeDamage(_projectileSpell.RuntimeData.Damage, hitPoint);
            }
        }

        private void CreateLink(LightningChainModifierSpell other)
        {
            GameObject linkObject = new($"LightningLink_{GetInstanceID()}_{other.GetInstanceID()}");
            linkObject.transform.SetParent(transform, false);

            LineRenderer lineRenderer = linkObject.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = true;
            lineRenderer.startWidth = _lineWidth;
            lineRenderer.endWidth = _lineWidth;
            lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lineRenderer.receiveShadows = false;

            if (_lineMaterial != null)
            {
                lineRenderer.material = _lineMaterial;
            }

            _activeLinks.Add(other.GetInstanceID(), new ActiveLink(other, lineRenderer, _damageInterval));
        }

        private void RemoveLink(LightningChainModifierSpell other)
        {
            if (other == null)
            {
                return;
            }

            int key = other.GetInstanceID();
            if (!_activeLinks.TryGetValue(key, out ActiveLink link))
            {
                return;
            }

            DestroyLink(link);
            _activeLinks.Remove(key);
        }

        private void ClearLinks()
        {
            foreach (ActiveLink link in _activeLinks.Values)
            {
                DestroyLink(link);
            }

            _activeLinks.Clear();
            _keysToRemove.Clear();
        }

        private void DestroyLink(ActiveLink link)
        {
            if (link.Renderer != null)
            {
                Destroy(link.Renderer.gameObject);
            }
        }

        private Vector3 GetSpellPosition()
        {
            return _projectileSpell.transform.position;
        }

        private static bool ShouldOwnLink(LightningChainModifierSpell a, LightningChainModifierSpell b)
        {
            return a.GetInstanceID() < b.GetInstanceID();
        }

        private readonly List<int> _keysToRemove = new();

        private sealed class ActiveLink
        {
            public LightningChainModifierSpell Other { get; }
            public LineRenderer Renderer { get; }
            public float DamageTimer { get; set; }

            public ActiveLink(LightningChainModifierSpell other, LineRenderer renderer, float damageTimer)
            {
                Other = other;
                Renderer = renderer;
                DamageTimer = damageTimer;
            }
        }
    }
}
