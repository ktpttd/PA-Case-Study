using DG.Tweening;
using DuetCats.Gameplay;
using DuetCats.Session;
using TMPro;
using UnityEngine;

namespace DuetCats.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ScoreState))]
    [RequireComponent(typeof(GameSession))]
    public sealed class ScorePresenter : MonoBehaviour
    {
        [SerializeField] private ScoreState scoreState;
        [SerializeField] private GameSession gameSession;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField, Min(1f)] private float outsideOffset = 240f;
        [SerializeField, Min(0.01f)] private float transitionDuration = 0.3f;

        private RectTransform scoreTransform;
        private Vector2 visiblePosition;
        private Vector2 hiddenPosition;
        private Tween movementTween;
        private Tween punchTween;

        private void Start()
        {
            if (scoreState == null || gameSession == null || scoreText == null)
            {
                Debug.LogError("ScorePresenter needs ScoreState, GameSession and a TextMeshProUGUI target.", this);
                enabled = false;
                return;
            }

            scoreTransform = scoreText.rectTransform;
            visiblePosition = scoreTransform.anchoredPosition;
            hiddenPosition = visiblePosition + Vector2.right * GetOutsideDirection() * outsideOffset;
            SetHiddenInstant();

            scoreState.ScoreChanged += UpdateScore;
            gameSession.Started += PlayEntrance;
            gameSession.Finished += PlayExit;
            UpdateScore(scoreState.Score);
        }

        private void OnDestroy()
        {
            if (scoreState != null)
            {
                scoreState.ScoreChanged -= UpdateScore;
            }

            if (gameSession != null)
            {
                gameSession.Started -= PlayEntrance;
                gameSession.Finished -= PlayExit;
            }

            movementTween.Kill();
            punchTween.Kill();
        }

        private void UpdateScore(int score)
        {
            if (scoreText == null)
            {
                return;
            }

            scoreText.SetText($"{score}");
            if (gameSession != null && gameSession.Phase == GamePhase.Playing)
            {
                punchTween.Kill();
                punchTween = scoreTransform.DOPunchScale(Vector3.one * 0.2f, 0.25f, 1, 1);
            }
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
            scoreTransform.anchoredPosition = hiddenPosition;
        }

        private void AnimateTo(Vector2 targetPosition, Ease ease)
        {
            movementTween.Kill();
            movementTween = scoreTransform
                .DOAnchorPos(targetPosition, transitionDuration)
                .SetEase(ease)
                .SetUpdate(true);
        }

        private float GetOutsideDirection()
        {
            var anchorCenterX = (scoreTransform.anchorMin.x + scoreTransform.anchorMax.x) * 0.5f;
            if (!Mathf.Approximately(anchorCenterX, 0.5f))
            {
                return anchorCenterX > 0.5f ? 1f : -1f;
            }

            return visiblePosition.x >= 0f ? 1f : -1f;
        }
    }
}