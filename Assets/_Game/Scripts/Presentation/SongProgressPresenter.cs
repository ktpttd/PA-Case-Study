using DG.Tweening;
using DuetCats.Content;
using DuetCats.Gameplay;
using DuetCats.Session;
using UnityEngine;
using UnityEngine.UI;

namespace DuetCats.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(GameSession))]
    [RequireComponent(typeof(NoteSystem))]
    public sealed class SongProgressPresenter : MonoBehaviour
    {
        [SerializeField] private GameSession gameSession;
        [SerializeField] private Image progressFillImage;
        [SerializeField] private RectTransform progressBarRoot;
        [SerializeField, Min(0f)] private float minFillWidth;
        [SerializeField, Min(1f)] private float outsideOffset = 160f;
        [SerializeField, Min(0.01f)] private float transitionDuration = 0.3f;

        private NoteSystem noteSystem;
        private int totalNoteCount;
        private int hitNoteCount;
        private RectTransform progressFillTransform;
        private float sceneMinFillWidth;
        private float fullFillWidth;
        private Vector2 visiblePosition;
        private Vector2 hiddenPosition;
        private Tween movementTween;
        private Tween progressTween;

        private void Start()
        {
            if (progressBarRoot == null && progressFillImage != null)
            {
                progressBarRoot = progressFillImage.rectTransform.parent as RectTransform;
            }

            noteSystem = GetComponent<NoteSystem>();
            if (gameSession == null || noteSystem == null || progressFillImage == null ||
                progressBarRoot == null || gameSession.SongContent == null)
            {
                Debug.LogError("SongProgressPresenter needs GameSession, NoteSystem, a progress Image, its RectTransform root and valid SongContent.", this);
                enabled = false;
                return;
            }

            totalNoteCount = gameSession.SongContent.Notes.Count;
            ConfigureFillForWidthProgress();
            visiblePosition = progressBarRoot.anchoredPosition;
            hiddenPosition = visiblePosition + Vector2.up * outsideOffset;
            SetHiddenInstant();

            gameSession.Started += PlayEntrance;
            gameSession.Finished += PlayExit;
            noteSystem.NoteHit += HandleNoteHit;
            RefreshProgress();
        }

        private void OnDestroy()
        {
            if (gameSession != null)
            {
                gameSession.Started -= PlayEntrance;
                gameSession.Finished -= PlayExit;
            }

            if (noteSystem != null)
            {
                noteSystem.NoteHit -= HandleNoteHit;
            }

            movementTween.Kill();
            progressTween.Kill();
        }

        private void RefreshProgress()
        {
            if (progressFillTransform == null || totalNoteCount <= 0)
            {
                return;
            }

            var progress = Mathf.Clamp01((float)hitNoteCount / totalNoteCount);
            var minimumWidth = Mathf.Min(
                Mathf.Max(minFillWidth, sceneMinFillWidth),
                fullFillWidth);
            var width = Mathf.Lerp(minimumWidth, fullFillWidth, progress);
            progressTween.Kill();
            progressTween = DOTween.To(
                    () => progressFillTransform.rect.width,
                    value => progressFillTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value),
                    width,
                    transitionDuration)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true);
        }

        private void HandleNoteHit(RuntimeNote note)
        {
            hitNoteCount++;
            RefreshProgress();
        }

        private void ConfigureFillForWidthProgress()
        {
            progressFillTransform = progressFillImage.rectTransform;
            sceneMinFillWidth = progressFillTransform.rect.width;
            fullFillWidth = Mathf.Max(
                sceneMinFillWidth,
                progressBarRoot.rect.width - progressFillTransform.anchoredPosition.x);
        }

        private void PlayEntrance()
        {
            AnimateTo(visiblePosition, Ease.OutBack);
        }

        private void PlayExit(GameOutcome outcome)
        {
            AnimateTo(hiddenPosition, Ease.InBack);
        }

        private void SetHiddenInstant()
        {
            movementTween.Kill();
            progressBarRoot.anchoredPosition = hiddenPosition;
        }

        private void AnimateTo(Vector2 targetPosition, Ease ease)
        {
            movementTween.Kill();
            movementTween = progressBarRoot
                .DOAnchorPos(targetPosition, transitionDuration)
                .SetEase(ease)
                .SetUpdate(true);
        }
    }
}
