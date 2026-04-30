using UnityEngine;

namespace ProjectDarkness
{
    public class BowManager : MonoBehaviour
    {
        public static BowManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }
        
        
    }
}
