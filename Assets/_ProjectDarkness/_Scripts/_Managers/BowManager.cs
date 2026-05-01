using System;
using UnityEngine;

namespace ProjectDarkness
{
    public class BowManager : MonoBehaviour
    {
        public static BowManager Instance { get; private set; }
        public event Action<float> OnBowReleased;
        
        [SerializeField] private float _chargeDuration = 1f;
        

        [Range(0f, 1f)] private float _chargePercent;
        public float ChargePercent => _chargePercent;
        public bool IsCharging => _isCharging;
        
        private bool _isCharging;
        private Timer _chargeTimer;

        private void Awake()
        {
            Instance = this;
            _chargeTimer = new Timer(Mathf.Max(_chargeDuration, 0.01f));
        }

        private void Update()
        {
            if (GameInput.Instance == null)
            {
                return;
            }

            if (GameInput.Instance.IsHoldingBowChargeDown)
            {
                UpdateCharge();
                return;
            }

            if (_isCharging || _chargePercent > 0f)
            {
                ReleaseBow();
            }
        }

        private void UpdateCharge()
        {
            if (!_isCharging)
            {
                StartCharge();
            }

            if (_chargePercent >= 1f)
            {
                _chargePercent = 1f;
                Debug.Log($"Bow Charge: {Mathf.RoundToInt(Mathf.Clamp01(_chargePercent) * 100f)}%");
                return;
            }

            _chargeTimer.Tick(Time.deltaTime);
            _chargePercent = Mathf.Clamp01(_chargeTimer.GetPercentComplete());
            Debug.Log($"Bow Charge: {Mathf.RoundToInt(Mathf.Clamp01(_chargePercent) * 100f)}%");
        }

        private void StartCharge()
        {
            _isCharging = true;
            _chargePercent = 0f;
            _chargeTimer.Reset();
            Debug.Log($"Bow Charge Started!");
        }

        private void ReleaseBow()
        {
            OnBowReleased?.Invoke(_chargePercent);
            OnRelease(_chargePercent);
            _isCharging = false;
            _chargePercent = 0f;
            _chargeTimer.Reset();
        }
        
        public void OnRelease(float chargeCompletion)
        {
            int chargePercentage = Mathf.RoundToInt(Mathf.Clamp01(chargeCompletion) * 100f);
            Debug.Log($"Bow released with {chargePercentage}% power!");
        }
    }
}
