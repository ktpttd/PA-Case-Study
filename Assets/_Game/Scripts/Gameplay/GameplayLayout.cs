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
        [SerializeField] private float judgementWorldY = -3.25f;

        [Header("Portrait logical X ranges")]
        [SerializeField, Range(0f, 1f)] private float leftMinX = 0.05f;
        [SerializeField, Range(0f, 1f)] private float leftMaxX = 0.45f;
        [SerializeField, Range(0f, 1f)] private float rightMinX = 0.55f;
        [SerializeField, Range(0f, 1f)] private float rightMaxX = 0.95f;

        [Header("Landscape logical X ranges")]
        [SerializeField, Range(0f, 1f)] private float landscapeLeftMinX = 0.20f;
        [SerializeField, Range(0f, 1f)] private float landscapeLeftMaxX = 0.45f;
        [SerializeField, Range(0f, 1f)] private float landscapeRightMinX = 0.55f;
        [SerializeField, Range(0f, 1f)] private float landscapeRightMaxX = 0.80f;

        [Header("Judgement")]
        [SerializeField, Range(0.01f, 0.49f)] private float maxCatchDistanceOfLaneSpacing = 0.45f;

        private float[] laneX;
        private int leftLaneCount;
        private bool initializedForLandscape;

        public bool IsInitialized { get { return laneX != null; } }
        public bool IsLandscape { get { return IsLandscapeScreen(); } }
        public bool HasWorldProjection { get { return ResolveGameplayCamera() != null; } }
        public float JudgementWorldY { get { return judgementWorldY; } }

        public bool Initialize(SongContent songContent)
        {
            if (songContent == null || !HasValidRanges())
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
                : Mathf.Min(configuredDistance, nearestLaneDistance * maxCatchDistanceOfLaneSpacing);
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
                minX = side == CatSide.Left ? landscapeLeftMinX : landscapeRightMinX;
                maxX = side == CatSide.Left ? landscapeLeftMaxX : landscapeRightMaxX;
                return;
            }

            minX = side == CatSide.Left ? leftMinX : rightMinX;
            maxX = side == CatSide.Left ? leftMaxX : rightMaxX;
        }

        private bool HasValidRanges()
        {
            return HasValidRangePair(leftMinX, leftMaxX, rightMinX, rightMaxX) &&
                   HasValidRangePair(
                       landscapeLeftMinX,
                       landscapeLeftMaxX,
                       landscapeRightMinX,
                       landscapeRightMaxX);
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
