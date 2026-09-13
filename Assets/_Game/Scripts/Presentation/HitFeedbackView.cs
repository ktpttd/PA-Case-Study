using DG.Tweening;
using UnityEngine;

namespace DuetCats.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMesh))]
    public sealed class HitFeedbackView : MonoBehaviour
    {
        [SerializeField] private TextMesh textMesh;

        private Vector3 initialLocalPosition;
        private Vector3 initialLocalScale;
        private Color initialColor;
        private Tween movementTween;
        private Tween scaleTween;
        private Tween fadeTween;

        private void Awake()
        {
            if (textMesh == null)
            {
                textMesh = GetComponent<TextMesh>();
            }

            initialLocalPosition = transform.localPosition;
            initialLocalScale = transform.localScale;
            if (textMesh != null)
            {
                initialColor = textMesh.color;
            }

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
            if (textMesh == null)
            {
                textMesh = GetComponent<TextMesh>();
            }

            if (textMesh == null || string.IsNullOrEmpty(message))
            {
                return;
            }

            gameObject.SetActive(true);
            KillTween(ref movementTween);
            KillTween(ref scaleTween);
            KillTween(ref fadeTween);

            transform.localPosition = initialLocalPosition;
            transform.localScale = initialLocalScale;
            textMesh.text = message;
            textMesh.color = initialColor;

            movementTween = transform
                .DOLocalMoveY(initialLocalPosition.y + riseDistance, duration)
                .SetEase(Ease.OutQuad);
            scaleTween = transform
                .DOScale(initialLocalScale * 0.5f, duration)
                .SetEase(Ease.InQuad);
            fadeTween = DOTween
                .ToAlpha(() => textMesh.color, color => textMesh.color = color, 0f, duration)
                .SetEase(Ease.InQuad)
                .OnComplete(Hide);
        }

        private void Hide()
        {
            if (textMesh != null)
            {
                textMesh.text = string.Empty;
            }

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
