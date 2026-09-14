using DG.Tweening;
using DuetCats.Session;
using UnityEngine;
using UnityEngine.UI;

namespace DuetCats.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(GameSession))]
    public sealed class SongProgressPresenter : MonoBehaviour
    {
        [SerializeField] private GameSession gameSession;
        [SerializeField] private Image progressFillImage;
        [SerializeField] private RectTransform progressBarRoot;
        [SerializeField, Min(0f)] private float minFillWidth;
        [SerializeField, Min(1f)] private float outsideOffset = 160f;
        [SerializeField, Min(0.01f)] private float transitionDuration = 0.3f;

        private float songDuration;
        private RectTransform progressFillTransform;
        private float sceneMinFillWidth;
        private float fullFillWidth;
        private Vector2 visiblePosition;
        private Vector2 hiddenPosition;
        private Tween movementTween;

        private void Start()
        {
            if (progressBarRoot == null && progressFillImage != null)
            {
                progressBarRoot = progressFillImage.rectTransform.parent as RectTransform;
            }

            if (gameSession == null || progressFillImage == null || progressBarRoot == null ||
                gameSession.SongConfig == null || gameSession.SongConfig.AudioClip == null)
            {
                Debug.LogError("SongProgressPresenter needs GameSession, a progress Image, its RectTransform root and an AudioClip.", this);
                enabled = false;
                return;
            }

            songDuration = gameSession.SongConfig.AudioClip.length;
            ConfigureFillForWidthProgress();
            visiblePosition = progressBarRoot.anchoredPosition;
            hiddenPosition = visiblePosition + Vector2.up * outsideOffset;
            SetHiddenInstant();

            gameSession.Started += PlayEntrance;
            gameSession.Finished += PlayExit;
            RefreshProgress();
        }

        private void Update()
        {
            RefreshProgress();
        }

        private void OnDestroy()
        {
            if (gameSession != null)
            {
                gameSession.Started -= PlayEntrance;
                gameSession.Finished -= PlayExit;
            }

            movementTween.Kill();
        }

        private void RefreshProgress()
        {
            if (progressFillTransform == null || songDuration <= 0f)
            {
                return;
            }

            var progress = Mathf.Clamp01(gameSession.SongTime / songDuration);
            var minimumWidth = Mathf.Min(
                Mathf.Max(minFillWidth, sceneMinFillWidth),
                fullFillWidth);
            var width = Mathf.Lerp(minimumWidth, fullFillWidth, progress);
            progressFillTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
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
