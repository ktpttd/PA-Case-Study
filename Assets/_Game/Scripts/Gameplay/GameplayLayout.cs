using System;
using DuetCats.Content;
using UnityEngine;

namespace DuetCats.Gameplay
{
    public enum CatSide
    {
        Left = 0,
        Right = 1
    }

    [DisallowMultipleComponent]
    public sealed class GameplayLayout : MonoBehaviour
    {
        [SerializeField] private Camera gameplayCamera;

        private float[] laneX;
        private int leftLaneCount;
        private bool initializedForLandscape;
        private LayoutTuning tuning;

        public bool IsInitialized { get { return laneX != null; } }
        public bool IsLandscape { get { return IsLandscapeScreen(); } }
        public bool HasWorldProjection { get { return ResolveGameplayCamera() != null; } }
        public float JudgementWorldY { get { return tuning.JudgementWorldY; } }
        public float SpawnWorldY { get { return tuning.SpawnWorldY; } }
        public LayoutTuning Tuning { get { return tuning; } }

        public bool Initialize(SongContent songContent, GlobalSetting globalSetting)
        {
            tuning = globalSetting == null ? null : globalSetting.Layout;
            if (songContent == null || tuning == null || !HasValidRanges())
            {
                return false;
            }

            var isLandscape = IsLandscape;
            if (laneX != null && laneX.Length == songContent.LaneCount &&
                initializedForLandscape == isLandscape)
            {
                return true;
            }

            laneX = new float[songContent.LaneCount];
            leftLaneCount = songContent.LeftLaneCount;
            GetRange(CatSide.Left, out var leftMin, out var leftMax);
            GetRange(CatSide.Right, out var rightMin, out var rightMax);
            FillLanePositions(0, songContent.LeftLaneCount, leftMin, leftMax);
            FillLanePositions(songContent.LeftLaneCount, songContent.RightLaneCount, rightMin, rightMax);
            initializedForLandscape = isLandscape;
            return true;
        }

        public float GetLaneX(int laneIndex)
        {
            if (laneX == null || laneIndex < 0 || laneIndex >= laneX.Length)
            {
                throw new ArgumentOutOfRangeException("laneIndex");
            }

            return laneX[laneIndex];
        }

        public float GetCatchDistance(int laneIndex, float configuredDistance)
        {
            if (laneX == null || laneIndex < 0 || laneIndex >= laneX.Length)
            {
                throw new ArgumentOutOfRangeException("laneIndex");
            }

            if (!IsLandscape)
            {
                return configuredDistance;
            }

            var firstLaneIndex = laneIndex < leftLaneCount ? 0 : leftLaneCount;
            var lastLaneIndex = laneIndex < leftLaneCount ? leftLaneCount - 1 : laneX.Length - 1;
            var nearestLaneDistance = float.MaxValue;

            if (laneIndex > firstLaneIndex)
            {
                nearestLaneDistance = Mathf.Min(nearestLaneDistance, laneX[laneIndex] - laneX[laneIndex - 1]);
            }

            if (laneIndex < lastLaneIndex)
            {
                nearestLaneDistance = Mathf.Min(nearestLaneDistance, laneX[laneIndex + 1] - laneX[laneIndex]);
            }

            return nearestLaneDistance == float.MaxValue
                ? configuredDistance
                : Mathf.Min(configuredDistance, nearestLaneDistance * tuning.MaxCatchDistanceOfLaneSpacing);
        }

        public float GetDefaultCatX(CatSide side)
        {
            GetRange(side, out var minX, out var maxX);
            return (minX + maxX) * 0.5f;
        }

        public float ClampCatX(CatSide side, float value)
        {
            GetRange(side, out var minX, out var maxX);
            return Mathf.Clamp(value, minX, maxX);
        }

        public float ToWorldX(float logicalX, Vector3 referenceWorldPosition)
        {
            var camera = ResolveGameplayCamera();
            if (camera == null)
            {
                throw new InvalidOperationException("GameplayLayout needs a gameplay camera.");
            }

            if (!camera.orthographic)
            {
                throw new InvalidOperationException("GameplayLayout world projection requires an orthographic camera.");
            }

            var viewportPosition = camera.WorldToViewportPoint(referenceWorldPosition);
            viewportPosition.x = Mathf.Clamp01(logicalX);
            return camera.ViewportToWorldPoint(viewportPosition).x;
        }

        private void FillLanePositions(int firstLaneIndex, int laneCount, float minX, float maxX)
        {
            if (laneCount == 1)
            {
                laneX[firstLaneIndex] = (minX + maxX) * 0.5f;
                return;
            }

            for (var index = 0; index < laneCount; index++)
            {
                var normalizedIndex = index / (float)(laneCount - 1);
                laneX[firstLaneIndex + index] = Mathf.Lerp(minX, maxX, normalizedIndex);
            }
        }

        private void GetRange(CatSide side, out float minX, out float maxX)
        {
            if (IsLandscape)
            {
                minX = side == CatSide.Left ? tuning.LandscapeLeftMinX : tuning.LandscapeRightMinX;
                maxX = side == CatSide.Left ? tuning.LandscapeLeftMaxX : tuning.LandscapeRightMaxX;
                return;
            }

            minX = side == CatSide.Left ? tuning.LeftMinX : tuning.RightMinX;
            maxX = side == CatSide.Left ? tuning.LeftMaxX : tuning.RightMaxX;
        }

        private bool HasValidRanges()
        {
            return HasValidRangePair(tuning.LeftMinX, tuning.LeftMaxX, tuning.RightMinX, tuning.RightMaxX) &&
                   HasValidRangePair(
                       tuning.LandscapeLeftMinX,
                       tuning.LandscapeLeftMaxX,
                       tuning.LandscapeRightMinX,
                       tuning.LandscapeRightMaxX);
        }

        private static bool HasValidRangePair(float leftMin, float leftMax, float rightMin, float rightMax)
        {
            return leftMin <= leftMax && rightMin <= rightMax && leftMax <= rightMin;
        }

        private static bool IsLandscapeScreen()
        {
            return Screen.width > Screen.height;
        }

        private Camera ResolveGameplayCamera()
        {
            if (gameplayCamera == null)
            {
                gameplayCamera = Camera.main;
            }

            return gameplayCamera;
        }
    }
}
