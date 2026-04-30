using UnityEngine;
using UnityEngine.UI;

namespace ProjectDarkness
{
    public class BowChargeUI : MonoBehaviour
    {
        [SerializeField] private GameObject _bowChargeIndicator;
        [SerializeField] private Image _bowChargeIcon;

        private void Start()
        {
            if (_bowChargeIcon != null)
            {
                _bowChargeIcon.fillAmount = 0f;
            }

            Hide();
        }

        private void Update()
        {
            if (BowManager.Instance == null)
            {
                Hide();
                return;
            }

            if (!BowManager.Instance.IsCharging)
            {
                if (_bowChargeIcon != null)
                {
                    _bowChargeIcon.fillAmount = 0f;
                }

                Hide();
                return;
            }

            Show();

            if (_bowChargeIcon != null)
            {
                _bowChargeIcon.fillAmount = BowManager.Instance.ChargePercent;
            }
        }

        private void Show()
        {
            if (_bowChargeIndicator != null)
            {
                _bowChargeIndicator.SetActive(true);
            }
        }
        
        private void Hide()
        {
            if (_bowChargeIndicator != null)
            {
                _bowChargeIndicator.SetActive(false);
            }
        }

    }
}
