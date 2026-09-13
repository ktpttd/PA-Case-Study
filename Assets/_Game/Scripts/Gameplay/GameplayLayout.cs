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

        [Header("Logical X ranges")]
        [SerializeField, Range(0f, 1f)] private float leftMinX = 0.05f;
        [SerializeField, Range(0f, 1f)] private float leftMaxX = 0.45f;
        [SerializeField, Range(0f, 1f)] private float rightMinX = 0.55f;
        [SerializeField, Range(0f, 1f)] private float rightMaxX = 0.95f;

        private float[] laneX;

        public bool IsInitialized { get { return laneX != null; } }
        public bool HasWorldProjection { get { return ResolveGameplayCamera() != null; } }

        public bool Initialize(SongContent songContent)
        {
            if (songContent == null || !HasValidRanges())
            {
                return false;
            }

            if (laneX != null && laneX.Length == songContent.LaneCount)
            {
                return true;
            }

            laneX = new float[songContent.LaneCount];
            FillLanePositions(0, songContent.LeftLaneCount, leftMinX, leftMaxX);
            FillLanePositions(songContent.LeftLaneCount, songContent.RightLaneCount, rightMinX, rightMaxX);
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

        public float GetDefaultCatX(CatSide side)
        {
            return (GetMinX(side) + GetMaxX(side)) * 0.5f;
        }

        public float ClampCatX(CatSide side, float value)
        {
            return Mathf.Clamp(value, GetMinX(side), GetMaxX(side));
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

        private float GetMinX(CatSide side)
        {
            return side == CatSide.Left ? leftMinX : rightMinX;
        }

        private float GetMaxX(CatSide side)
        {
            return side == CatSide.Left ? leftMaxX : rightMaxX;
        }

        private bool HasValidRanges()
        {
            return leftMinX <= leftMaxX &&
                   rightMinX <= rightMaxX &&
                   leftMaxX <= rightMinX;
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
