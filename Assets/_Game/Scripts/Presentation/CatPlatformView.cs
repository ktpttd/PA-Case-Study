using DuetCats.Gameplay;
using UnityEngine;

namespace DuetCats.Presentation
{
    [DefaultExecutionOrder(75)]
    [DisallowMultipleComponent]
    public sealed class CatPlatformView : MonoBehaviour
    {
        [SerializeField] private CatSide side;
        [SerializeField] private GameplayLayout gameplayLayout;

        private void Start()
        {
            if (gameplayLayout == null || !gameplayLayout.HasWorldProjection ||
                gameplayLayout.Tuning == null)
            {
                Debug.LogError("CatPlatformView needs a GameplayLayout with an orthographic gameplay camera.", this);
                enabled = false;
                return;
            }

            var worldPosition = transform.position;
            var tuning = gameplayLayout.Tuning;
            var logicalXOffset = side == CatSide.Left
                ? tuning.LeftPlatformLogicalXOffset
                : tuning.RightPlatformLogicalXOffset;
            var logicalX = gameplayLayout.ClampCatX(
                side,
                gameplayLayout.GetDefaultCatX(side) + logicalXOffset);
            worldPosition.x = gameplayLayout.ToWorldX(logicalX, worldPosition);
            worldPosition.y = gameplayLayout.JudgementWorldY + tuning.PlatformJudgementWorldYOffset;
            transform.position = worldPosition;
        }
    }
}
