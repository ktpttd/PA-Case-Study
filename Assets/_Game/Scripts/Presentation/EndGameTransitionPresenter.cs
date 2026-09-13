using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace DuetCats.Presentation
{
    [DisallowMultipleComponent]
    public sealed class EndGameTransitionPresenter : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private GameObject transitionRoot;
        [SerializeField] private Image blackOverlay;
        [SerializeField] private Image pawImage;

        [Header("Layout")]
        [SerializeField] private Vector2 portraitPawSize = new Vector2(250f, 250f);
        [SerializeField] private Vector2 landscapePawSize = new Vector2(180f, 180f);

        [Header("Timing")]
        [SerializeField, Min(0.01f)] private float coverDuration = 0.25f;
        [SerializeField, Min(0f)] private float coveredHoldDuration = 0.1f;
        [SerializeField, Min(0.01f)] private float revealDuration = 0.35f;

        private Sequence transitionSequence;

        public bool IsReady
        {
            get { return transitionRoot != null && blackOverlay != null && pawImage != null; }
        }

        public float RevealDuration { get { return revealDuration; } }

        public void Play(Action onCovered, Action onReveal, Action onComplete)
        {
            if (!IsReady)
            {
                Debug.LogError("EndGameTransitionPresenter needs Transition Root, Black Overlay and Paw Image.", this);
                return;
            }

            transitionSequence.Kill();
            transitionRoot.SetActive(true);
            SetImageAlpha(blackOverlay, 0f);
            pawImage.color = Color.white;
            pawImage.rectTransform.sizeDelta = Screen.width > Screen.height
                ? landscapePawSize
                : portraitPawSize;
            pawImage.rectTransform.localScale = Vector3.one * 0.1f;

            transitionSequence = DOTween.Sequence()
                .SetUpdate(true)
                .Append(blackOverlay.DOFade(1f, coverDuration))
                .Join(pawImage.rectTransform.DOScale(1f, coverDuration).SetEase(Ease.OutBack))
                .AppendInterval(coveredHoldDuration)
                .AppendCallback(() => onCovered?.Invoke())
                .AppendCallback(() => onReveal?.Invoke())
                .Append(blackOverlay.DOFade(0f, revealDuration))
                .Join(pawImage.rectTransform.DOScale(0.15f, revealDuration).SetEase(Ease.InBack))
                .Join(pawImage.DOFade(0f, revealDuration * 0.7f))
                .AppendCallback(() =>
                {
                    transitionRoot.SetActive(false);
                    transitionSequence = null;
                    onComplete?.Invoke();
                });
        }

        private void OnDestroy()
        {
            transitionSequence.Kill();
        }

        private static void SetImageAlpha(Graphic image, float alpha)
        {
            var color = image.color;
            color.a = alpha;
            image.color = color;
        }
    }
}
