using UnityEngine;
using UnityEngine.UI;

namespace ProjectDarkness
{
    public class ManaUI : MonoBehaviour
    {
        [SerializeField] private Image _manaBarForground;

        private Wand _subscribedWand;

        private void Start()
        {
            SubscribeToCurrentWand();
            RefreshManaBar();
        }

        private void OnDestroy()
        {
            UnsubscribeFromCurrentWand();
        }

        private void SubscribeToCurrentWand()
        {
            UnsubscribeFromCurrentWand();

            _subscribedWand = WandManager.Instance.CurrentWand;
            _subscribedWand.OnManaUpdated += HandleManaUpdated;
        }

        private void UnsubscribeFromCurrentWand()
        {
            if (_subscribedWand == null)
            {
                return;
            }

            _subscribedWand.OnManaUpdated -= HandleManaUpdated;
            _subscribedWand = null;
        }

        private void HandleManaUpdated()
        {
            RefreshManaBar();
        }

        private void RefreshManaBar()
        {
            if (_manaBarForground == null)
            {
                return;
            }

            Wand currentWand = WandManager.Instance != null ? WandManager.Instance.CurrentWand : null;
            if (currentWand == null || currentWand.WandData == null || currentWand.WandData.ManaAmount <= 0f)
            {
                _manaBarForground.fillAmount = 0f;
                return;
            }

            _manaBarForground.fillAmount = currentWand.CurrentMana / currentWand.WandData.ManaAmount;
        }
    }
}
