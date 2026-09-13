using DuetCats.Content;
using DuetCats.Gameplay;
using UnityEngine;

namespace DuetCats.Presentation
{
    [DisallowMultipleComponent]
    public sealed class NoteView : MonoBehaviour
    {
        [SerializeField] private float spawnWorldY = 7f;
        [SerializeField] private float hitWorldY = -3f;

        private RuntimeNote note;
        private GameplayLayout gameplayLayout;
        private float logicalX;

        public RuntimeNote Note { get { return note; } }

        public void Show(RuntimeNote nextNote, float nextLogicalX, GameplayLayout nextLayout)
        {
            note = nextNote;
            logicalX = nextLogicalX;
            gameplayLayout = nextLayout;
            SetProgress(0f);
        }

        public void SetProgress(float progress)
        {
            if (gameplayLayout == null || !gameplayLayout.HasWorldProjection)
            {
                return;
            }

            var worldPosition = transform.position;
            worldPosition.y = Mathf.Lerp(spawnWorldY, hitWorldY, Mathf.Clamp01(progress));
            worldPosition.x = gameplayLayout.ToWorldX(logicalX, worldPosition);
            transform.position = worldPosition;
        }

        public void ResetView()
        {
            note = null;
            gameplayLayout = null;
            logicalX = 0f;
        }
    }
}
