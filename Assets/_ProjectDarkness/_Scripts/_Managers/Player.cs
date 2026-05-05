using System;
using UnityEngine;

namespace ProjectDarkness
{
    [RequireComponent(typeof(PlayerLook))]
    [RequireComponent(typeof(PlayerMovement))]
    public class Player : MonoBehaviour
    {
        public static Player Instance { get; private set; }
        public PlayerMovement Movement { get; private set; }

        private void Awake()
        {
            Instance = this;
            Movement = GetComponent<PlayerMovement>();
        }


    }
}
