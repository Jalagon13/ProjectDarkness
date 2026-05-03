using UnityEngine;

namespace ProjectDarkness
{
    public class WandManager : MonoBehaviour
    {
        public static WandManager Instance { get; private set; }
        
        [field: SerializeField] public Wand CurrentWand { get; private set; }

        private void Awake()
        {
            Instance = this;
        }
    }
}
