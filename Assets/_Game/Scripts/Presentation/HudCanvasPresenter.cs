using DG.Tweening;
using DuetCats.Session;
using UnityEngine;

namespace DuetCats.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class HudCanvasPresenter : MonoBehaviour
    {
        [SerializeField] private GameSession gameSession;
        [SerializeField] private CanvasGroup hudCanvasGroup;
        [SerializeField, Range(0f, 1f)] private float visibleAlpha = 1f;
        [SerializeField, Min(0.01f)] private float fadeDuration = 0.3f;

        private Tween fadeTween;

        private void Awake()
        {
            if (hudCanvasGroup == null)
            {
                hudCanvasGroup = GetComponent<CanvasGroup>();
            }
        }

        private void Start()
        {
            if (gameSession == null || hudCanvasGroup == null)
            {
                Debug.LogError("HudCanvasPresenter needs GameSession and a CanvasGroup on the HUD Canvas.", this);
                enabled = false;
                return;
            }

            hudCanvasGroup.alpha = 0f;
            gameSession.Started += FadeIn;
            gameSession.Finished += FadeOut;
        }

        private void OnDestroy()
        {
            if (gameSession != null)
            {
                gameSession.Started -= FadeIn;
                gameSession.Finished -= FadeOut;
            }

            fadeTween.Kill();
        }

        private void FadeIn()
        {
            FadeTo(visibleAlpha, Ease.OutQuad);
        }

        private void FadeOut(GameOutcome outcome)
        {
            FadeTo(0f, Ease.InQuad);
        }

        private void FadeTo(float targetAlpha, Ease ease)
        {
            fadeTween.Kill();
            fadeTween = hudCanvasGroup
                .DOFade(targetAlpha, fadeDuration)
                .SetEase(ease)
                .SetUpdate(true);
        }
    }
}
