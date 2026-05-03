using UnityEngine;
using UnityEngine.UI;

namespace ProjectDarkness
{
    public class CooldownUI : MonoBehaviour
    {
        [SerializeField] private RectTransform _bar;
        [SerializeField] private Image _cooldownBarForground;
        
        private void Awake()
        {
            Hide();
        }
        private void Update()
        {
            if (WandManager.Instance == null || WandManager.Instance.CurrentWand == null)
            {
                Hide();
                return;
            }

            Timer cooldownTimer = WandManager.Instance.CurrentWand.CooldownTimer;

            if (cooldownTimer != null && cooldownTimer.IsRunning())
            {
                Show();
                _cooldownBarForground.fillAmount = cooldownTimer.GetPercentComplete();
            }
            else
            {
                Hide();
            }
        }
        private void Show()
        {
            _bar.gameObject.SetActive(true);
        }
        
        private void Hide()
        {
            _bar.gameObject.SetActive(false);
        }
    }
}
