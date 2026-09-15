using DG.Tweening;
using TMPro;
using UnityEngine;

namespace DuetCats.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMeshPro))]
    public sealed class HitFeedbackView : MonoBehaviour
    {
        [SerializeField] private TextMeshPro feedbackText;

        private Vector3 initialLocalPosition;
        private Vector3 initialLocalScale;
        private Color initialColor;
        private Tween movementTween;
        private Tween scaleTween;
        private Tween fadeTween;

        private void Awake()
        {
            initialLocalPosition = transform.localPosition;
            initialLocalScale = transform.localScale;
            initialColor = feedbackText.color;

            Hide();
        }

        private void OnDisable()
        {
            KillTween(ref movementTween);
            KillTween(ref scaleTween);
            KillTween(ref fadeTween);
        }

        public void Show(string message, float duration, float riseDistance)
        {
            if (feedbackText == null || string.IsNullOrEmpty(message))
            {
                return;
            }

            gameObject.SetActive(true);
            KillTween(ref movementTween);
            KillTween(ref scaleTween);
            KillTween(ref fadeTween);

            transform.localPosition = initialLocalPosition;
            transform.localScale = initialLocalScale;
            feedbackText.SetText(message);
            feedbackText.color = initialColor;

            movementTween = transform
                .DOLocalMoveY(initialLocalPosition.y + riseDistance, duration)
                .SetEase(Ease.InQuart);
            scaleTween = transform
                .DOScale(initialLocalScale * 0.5f, duration)
                .SetEase(Ease.InQuad);
            fadeTween = DOTween
                .ToAlpha(() => feedbackText.color, color => feedbackText.color = color, 0f, duration)
                .SetEase(Ease.InQuad)
                .OnComplete(Hide);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private static void KillTween(ref Tween tween)
        {
            if (tween == null)
            {
                return;
            }

            tween.Kill();
            tween = null;
        }
    }
}
