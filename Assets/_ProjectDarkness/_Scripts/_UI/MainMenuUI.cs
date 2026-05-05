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
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                
                GameManager.Instance.GameStarted = true;

                Loader.Load(Loader.Scene.GameScene);
            });

            _quitButton.onClick.AddListener(() =>
            {
                Application.Quit();
            });

            Time.timeScale = 1f;
        }
    }
}
