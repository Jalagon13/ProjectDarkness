using System;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectDarkness
{
    public class PauseMenuUI : MonoBehaviour
    {
        [SerializeField] private Button _exitToTitleButton;
        [SerializeField] private RectTransform _pauseMenu;
        
        private bool _isPaused;
        
        private void Awake()
        {
            _exitToTitleButton.onClick.AddListener(() =>
            {
                Time.timeScale = 1f;
                // Scene loading stuff here
                Loader.Load(Loader.Scene.MainMenuScene);
            });
        }
        
        private void Start()
        {
            GameInput.Instance.OnTogglePauseMenu += TogglePauseMenu;
            
            Hide();
        }
        
        private void OnDestroy()
        {
            GameInput.Instance.OnTogglePauseMenu -= TogglePauseMenu;
        }

        private void TogglePauseMenu()
        {
            if(InventoryManager.Instance.InventoryUI.IsOpen) return;
        
            _isPaused = !_isPaused;

            if (_isPaused)
            {
                Show();
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Hide();
                Time.timeScale = 1f;
                // Be careful: this might conflict with InventoryUI cursor logic
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void Show()
        {
            _pauseMenu.gameObject.SetActive(true);
        }
        
        private void Hide()
        {
            _pauseMenu.gameObject.SetActive(false);
        }
    }
}
