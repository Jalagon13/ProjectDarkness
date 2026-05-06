using FMOD.Studio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectDarkness
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _quitButton;

        private void Awake()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            _newGameButton.onClick.AddListener(() =>
            {
                if(GameManager.Instance.GameRestarted) return; // If player chose restart, do not be able to press this button and let the game restart by itself
            
                EnterGameScene();
            });

            _quitButton.onClick.AddListener(() =>
            {
                Application.Quit();
            });

            Time.timeScale = 1f;
        }
        
        private void Start()
        {
            if(GameManager.Instance.GameRestarted)
            {
                // Automatically start a new run
                GameManager.Instance.GameRestarted = false;
                EnterGameScene();
            }
            
            GameManager.Instance.GameComplete = false;
        }
        
        private void EnterGameScene()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            GameManager.Instance.GameStarted = true;

            Loader.Load(Loader.Scene.GameScene);
        }
    }
}
