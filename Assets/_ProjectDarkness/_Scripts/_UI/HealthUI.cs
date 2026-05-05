using System;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectDarkness
{
    public class HealthUI : MonoBehaviour
    {
        [SerializeField] private Image _healthFillImage;
        [SerializeField] private GameObject _deathPanel;
        
        private void Start()
        {
            HideDeathPanel();

            if (HealthManager.Instance != null)
            {
                HealthManager.Instance.OnHealthChanged += UpdateHealthBar;
                HealthManager.Instance.OnDeath += ShowDeathPanel;
                UpdateHealthBar();
            }
        }

        private void OnDestroy()
        {
            if (HealthManager.Instance != null)
            {
                HealthManager.Instance.OnHealthChanged -= UpdateHealthBar;
                HealthManager.Instance.OnDeath -= ShowDeathPanel;
            }
        }

        private void UpdateHealthBar()
        {
            if (_healthFillImage != null && HealthManager.Instance.MaxHealth > 0)
            {
                float fillAmount = (float)HealthManager.Instance.CurrentHealth / HealthManager.Instance.MaxHealth;
                _healthFillImage.fillAmount = fillAmount;
            }
        }

        private void ShowDeathPanel()
        {
            _deathPanel.SetActive(true);
            
            GameInput.Instance.OnTogglePauseMenu += ExitGame;
            GameInput.Instance.OnJump += RestartRun;
        }

        private void HideDeathPanel()
        {
            _deathPanel.SetActive(false);

            GameInput.Instance.OnTogglePauseMenu -= ExitGame;
            GameInput.Instance.OnJump -= RestartRun;
        }

        private void ExitGame()
        {
            Time.timeScale = 1f;
            // Scene loading stuff here
            Loader.Load(Loader.Scene.MainMenuScene);
        }

        private void RestartRun()
        {
            HideDeathPanel();
            Time.timeScale = 1f;
            GameManager.Instance.GameRestarted = true;
            Debug.Log($"Restarting Run {GameManager.Instance.GameRestarted}");
            Loader.Load(Loader.Scene.MainMenuScene);
        }
    }
}
