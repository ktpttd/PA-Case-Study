using DuetCats.Gameplay;
using UnityEngine;

namespace DuetCats.Presentation
{
    [DisallowMultipleComponent]
    public sealed class CatPlatformView : MonoBehaviour
    {
        [SerializeField] private CatSide side;
        [SerializeField] private GameplayLayout gameplayLayout;
        [SerializeField] private float logicalXOffset;
        [SerializeField] private float judgementWorldYOffset = -0.45f;

        private void Start()
        {
            if (gameplayLayout == null || !gameplayLayout.HasWorldProjection)
            {
                Debug.LogError("CatPlatformView needs a GameplayLayout with an orthographic gameplay camera.", this);
                enabled = false;
                return;
            }

            var worldPosition = transform.position;
            var logicalX = gameplayLayout.ClampCatX(
                side,
                gameplayLayout.GetDefaultCatX(side) + logicalXOffset);
            worldPosition.x = gameplayLayout.ToWorldX(logicalX, worldPosition);
            worldPosition.y = gameplayLayout.JudgementWorldY + judgementWorldYOffset;
            transform.position = worldPosition;
        }
    }
}
