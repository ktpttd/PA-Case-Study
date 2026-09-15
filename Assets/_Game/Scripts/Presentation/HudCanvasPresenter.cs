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
        private Tween fadeTween;

        private void Start()
        {
            if (gameSession == null || gameSession.GlobalSetting == null || hudCanvasGroup == null)
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
            FadeTo(gameSession.GlobalSetting.Hud.VisibleAlpha, Ease.OutQuad);
        }

        private void FadeOut(GameOutcome outcome)
        {
            FadeTo(0f, Ease.InQuad);
        }

        private void FadeTo(float targetAlpha, Ease ease)
        {
            fadeTween.Kill();
            fadeTween = hudCanvasGroup
                .DOFade(targetAlpha, gameSession.GlobalSetting.Hud.FadeDuration)
                .SetEase(ease)
                .SetUpdate(true);
        }
    }
}
