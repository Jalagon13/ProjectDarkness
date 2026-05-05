using UnityEngine;

namespace ProjectDarkness
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [HideInInspector]
        public bool GameStarted = false;

        [HideInInspector]
        public bool GameRestarted = false;

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
    }
}
