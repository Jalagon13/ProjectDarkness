using System;
using UnityEngine;

namespace ProjectDarkness
{
    [RequireComponent(typeof(PlayerLook))]
    [RequireComponent(typeof(PlayerMovement))]
    public class Player : MonoBehaviour
    {
        public static Player Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }


    }
}
