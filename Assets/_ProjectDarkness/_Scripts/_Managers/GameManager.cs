using System;
using UnityEngine;

namespace ProjectDarkness
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        public event Action OnGameComplete;
        
        [HideInInspector]
        public bool GameStarted = false;

        [HideInInspector]
        public bool GameRestarted = false;

        [HideInInspector]
        public bool GameComplete = false;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(this);
        }
        
        public void CompleteGame()
        {
            GameComplete = true;
            OnGameComplete?.Invoke();
        }
    }
}
