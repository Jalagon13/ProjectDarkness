using UnityEngine;

namespace ProjectDarkness
{
    public class RoomInfoIconUI : MonoBehaviour
    {
        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void LateUpdate()
        {
            if (Player.Instance == null) return;

            // Get the player's Y rotation (around the vertical axis)
            float playerRotationY = Player.Instance.transform.eulerAngles.y;

            // Map the player's world rotation to the UI's Z rotation.
            // In Unity UI, 0 rotation is 'Up' (Y+). Since Y rotation increases clockwise, 
            // we apply -playerRotationY to the Z axis to align the icon's 'Up' with the player's heading.
            _rectTransform.localRotation = Quaternion.Euler(0, 0, -playerRotationY);
        }
    }
}
