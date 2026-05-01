using UnityEngine;

namespace ProjectDarkness
{
    public class BowVisualsHandler : MonoBehaviour
    {
        [Header("Bow Frames")]
        [SerializeField] private GameObject _neutralFrame;
        [Space(5)]
        [SerializeField] private GameObject[] _bowFrames;
        [Space(5)]
        [SerializeField] private GameObject _fullyChargedFrame;

        private int _currentFrameIndex = -1;

        private void Start()
        {
            ShowNeutralFrame();
        }

        private void Update()
        {
            if (BowManager.Instance == null)
            {
                ShowNeutralFrame();
                return;
            }

            if (!BowManager.Instance.IsCharging)
            {
                ShowNeutralFrame();
                return;
            }

            ShowChargedFrame(BowManager.Instance.ChargePercent);
        }

        private void ShowNeutralFrame()
        {
            SetActiveFrameIndex(0);
        }

        private void ShowChargedFrame(float chargePercent)
        {
            int chargedFrameCount = GetChargedFrameCount();
            if (chargedFrameCount <= 0)
            {
                ShowNeutralFrame();
                return;
            }

            int chargedFrameIndex = GetChargedFrameIndexFromCharge(chargePercent, chargedFrameCount);
            SetActiveFrameIndex(chargedFrameIndex + 1);
        }

        private int GetTotalFrameCount()
        {
            return 2 + (_bowFrames?.Length ?? 0);
        }

        private int GetChargedFrameCount()
        {
            return GetTotalFrameCount() - 1;
        }

        private static int GetChargedFrameIndexFromCharge(float chargePercent, int chargedFrameCount)
        {
            if (chargedFrameCount <= 1)
            {
                return 0;
            }

            float clampedChargePercent = Mathf.Clamp01(chargePercent);
            return Mathf.Clamp(Mathf.FloorToInt(clampedChargePercent * chargedFrameCount), 0, chargedFrameCount - 1);
        }

        private void SetActiveFrameIndex(int activeFrameIndex)
        {
            if (_currentFrameIndex == activeFrameIndex)
            {
                return;
            }

            _currentFrameIndex = activeFrameIndex;
            SetOnlyFrameActive(_currentFrameIndex);
        }

        private void SetOnlyFrameActive(int activeFrameIndex)
        {
            SetFrameActive(_neutralFrame, activeFrameIndex == 0);

            int middleFrameCount = _bowFrames?.Length ?? 0;
            for (int i = 0; i < middleFrameCount; i++)
            {
                SetFrameActive(_bowFrames[i], activeFrameIndex == i + 1);
            }

            SetFrameActive(_fullyChargedFrame, activeFrameIndex == GetTotalFrameCount() - 1);
        }

        private static void SetFrameActive(GameObject frame, bool isActive)
        {
            if (frame != null && frame.activeSelf != isActive)
            {
                frame.SetActive(isActive);
            }
        }
    }
}
