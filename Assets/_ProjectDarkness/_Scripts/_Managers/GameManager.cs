using UnityEngine;

namespace ProjectDarkness
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [HideInInspector]
        public bool GameStarted = false;
        
        private void Awake()
        {
            Instance = this;

            DontDestroyOnLoad(this);
        }
    }
}
