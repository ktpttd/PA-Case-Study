using DuetCats.Content;
using DuetCats.Gameplay;
using UnityEngine;

namespace DuetCats.Presentation
{
    [DisallowMultipleComponent]
    public sealed class NoteView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        private RuntimeNote note;
        private Sprite defaultSprite;
        private GameplayLayout gameplayLayout;
        private float logicalX;

        public RuntimeNote Note { get { return note; } }

        private void Awake()
        {
            if (spriteRenderer != null)
            {
                defaultSprite = spriteRenderer.sprite;
            }
        }

        public void SetSprite(Sprite sprite)
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (spriteRenderer == null)
            {
                throw new System.InvalidOperationException("NoteView needs a SpriteRenderer.");
            }

            spriteRenderer.sprite = sprite;
        }

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
            worldPosition.y = Mathf.Lerp(
                gameplayLayout.SpawnWorldY,
                gameplayLayout.JudgementWorldY,
                Mathf.Clamp01(progress));
            worldPosition.x = gameplayLayout.ToWorldX(logicalX, worldPosition);
            transform.position = worldPosition;
        }

        public float GetProgressFromCurrentPosition()
        {
            if (gameplayLayout == null || !gameplayLayout.HasWorldProjection)
            {
                return 0f;
            }

            return Mathf.InverseLerp(
                gameplayLayout.SpawnWorldY,
                gameplayLayout.JudgementWorldY,
                transform.position.y);
        }

        public void ResetView()
        {
            note = null;
            gameplayLayout = null;
            logicalX = 0f;
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = defaultSprite;
            }
        }
    }
}
