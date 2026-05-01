using UnityEngine;

namespace ProjectDarkness
{
    public class BowVisualsHandler : MonoBehaviour
    {
        private const string ArrowHolderTag = "ArrowHolder";

        [Header("Bow Frames")]
        [SerializeField] private GameObject _neutralFrame;
        [Space(5)]
        [SerializeField] private GameObject[] _bowFrames;
        [Space(5)]
        [SerializeField] private GameObject _fullyChargedFrame;
        
        [Header("Arrow Settings")]
        [SerializeField] private StandardArrow _standardArrowPrefab;

        private int _currentFrameIndex = -1;
        private StandardArrow _spawnedArrow;

        private void Start()
        {
            ShowNeutralFrame();
        }

        private void Update()
        {
            if (BowManager.Instance == null)
            {
                ShowNeutralFrame();
                DestroySpawnedArrow();
                return;
            }

            if (!BowManager.Instance.IsCharging)
            {
                ShowNeutralFrame();
                DestroySpawnedArrow();
                return;
            }

            ShowChargedFrame(BowManager.Instance.ChargePercent);
            EnsureArrowSpawned();
            AlignArrowToCurrentFrame();
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

        private void EnsureArrowSpawned()
        {
            if (_spawnedArrow != null || _standardArrowPrefab == null)
            {
                return;
            }

            _spawnedArrow = Instantiate(_standardArrowPrefab, transform);
        }

        private void DestroySpawnedArrow()
        {
            if (_spawnedArrow == null)
            {
                return;
            }

            Destroy(_spawnedArrow.gameObject);
            _spawnedArrow = null;
        }

        private void AlignArrowToCurrentFrame()
        {
            if (_spawnedArrow == null)
            {
                return;
            }

            Transform arrowRearPoint = _spawnedArrow.RearPoint;
            Transform arrowHolder = GetArrowHolderForCurrentFrame();
            if (arrowRearPoint == null || arrowHolder == null)
            {
                return;
            }

            Transform arrowTransform = _spawnedArrow.transform;
            Quaternion rotationOffset = arrowHolder.rotation * Quaternion.Inverse(arrowRearPoint.rotation);
            arrowTransform.rotation = rotationOffset * arrowTransform.rotation;
            arrowTransform.position += arrowHolder.position - arrowRearPoint.position;
        }

        private Transform GetArrowHolderForCurrentFrame()
        {
            GameObject currentFrame = GetFrameByIndex(_currentFrameIndex);
            if (currentFrame == null)
            {
                return null;
            }

            Transform[] childTransforms = currentFrame.GetComponentsInChildren<Transform>(true);
            foreach (Transform childTransform in childTransforms)
            {
                if (childTransform.CompareTag(ArrowHolderTag))
                {
                    return childTransform;
                }
            }

            return null;
        }

        private GameObject GetFrameByIndex(int frameIndex)
        {
            if (frameIndex <= 0)
            {
                return _neutralFrame;
            }

            int middleFrameCount = _bowFrames?.Length ?? 0;
            if (frameIndex <= middleFrameCount)
            {
                return _bowFrames[frameIndex - 1];
            }

            return _fullyChargedFrame;
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

        private void OnDestroy()
        {
            DestroySpawnedArrow();
        }
    }
}
