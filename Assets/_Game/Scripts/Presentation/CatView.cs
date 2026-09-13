using DuetCats.Controls;
using DuetCats.Gameplay;
using UnityEngine;

namespace DuetCats.Presentation
{
    [DisallowMultipleComponent]
    public sealed class CatView : MonoBehaviour
    {
        [SerializeField] private CatSide side;
        [SerializeField] private CatInput catInput;
        [SerializeField] private GameplayLayout gameplayLayout;

        private void LateUpdate()
        {
            if (catInput == null || gameplayLayout == null || !gameplayLayout.HasWorldProjection)
            {
                return;
            }

            var logicalX = side == CatSide.Left ? catInput.LeftCatX : catInput.RightCatX;
            var worldPosition = transform.position;
            worldPosition.x = gameplayLayout.ToWorldX(logicalX, worldPosition);
            transform.position = worldPosition;
        }
    }
}
