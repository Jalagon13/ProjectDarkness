using UnityEngine;

namespace ProjectDarkness
{
    public class StandardArrow : MonoBehaviour
    {
        [field: SerializeField] private Transform ArrowRearPoint;

        public Transform RearPoint => ArrowRearPoint;
    }
}
