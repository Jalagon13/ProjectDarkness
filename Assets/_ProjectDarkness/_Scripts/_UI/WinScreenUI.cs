using System;
using UnityEngine;

namespace ProjectDarkness
{
    public class WinScreenUI : MonoBehaviour
    {
        [SerializeField] private RectTransform _winScreen;
    
        private void Start()
        {
            if(GameManager.Instance != null)
            {
                GameManager.Instance.OnGameComplete += CompleteGame;
            }
            
            Hide();
        }
        
        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameComplete -= CompleteGame;
            }
        }

        private void CompleteGame()
        {
            Show();
        }

        private void ExitGame()
        {
            Time.timeScale = 1f;
            // Scene loading stuff here
            Loader.Load(Loader.Scene.MainMenuScene);
        }

        private void RestartRun()
        {
            Hide();
            Time.timeScale = 1f;
            GameManager.Instance.GameRestarted = true;
            GameManager.Instance.GameComplete = false;
            Debug.Log($"Restarting Run {GameManager.Instance.GameRestarted}");
            Loader.Load(Loader.Scene.MainMenuScene);
        }

        private void Show()
        {
            _winScreen.gameObject.SetActive(true);

            GameInput.Instance.OnTogglePauseMenu += ExitGame;
            GameInput.Instance.OnJump += RestartRun;
        }
        
        private void Hide()
        {
            _winScreen.gameObject.SetActive(false);

            GameInput.Instance.OnTogglePauseMenu -= ExitGame;
            GameInput.Instance.OnJump -= RestartRun;
        }
    }
}
