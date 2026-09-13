using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace DuetCats.Presentation
{
    [DisallowMultipleComponent]
    public sealed class EndGameTransitionPresenter : MonoBehaviour
    {
        private static readonly int RadiusProperty = Shader.PropertyToID("_Radius");
        private static readonly int FeatherProperty = Shader.PropertyToID("_Feather");
        private static readonly int AspectProperty = Shader.PropertyToID("_Aspect");

        [Header("Scene References")]
        [SerializeField] private GameObject transitionRoot;
        [SerializeField] private Image blackOverlay;
        [SerializeField] private Image pawImage;
        [SerializeField] private Material irisMaterial;

        [Header("Iris")]
        [SerializeField, Min(0.1f)] private float openRadius = 1.2f;
        [SerializeField, Range(0.001f, 0.1f)] private float irisFeather = 0.015f;

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
            get { return transitionRoot != null && blackOverlay != null && pawImage != null && irisMaterial != null; }
        }

        public float RevealDuration { get { return revealDuration; } }

        public void Play(Action onCovered, Action onReveal, Action onComplete)
        {
            if (!IsReady)
            {
                Debug.LogError("EndGameTransitionPresenter needs Transition Root, Black Overlay, Paw Image and Iris Material.", this);
                return;
            }

            transitionSequence.Kill();
            transitionRoot.SetActive(true);
            blackOverlay.material = irisMaterial;
            irisMaterial.SetFloat(AspectProperty, (float)Screen.width / Screen.height);
            irisMaterial.SetFloat(FeatherProperty, irisFeather);
            SetIrisRadius(openRadius);
            pawImage.color = Color.white;
            pawImage.rectTransform.sizeDelta = Screen.width > Screen.height
                ? landscapePawSize
                : portraitPawSize;
            pawImage.rectTransform.localScale = Vector3.one * 0.1f;

            transitionSequence = DOTween.Sequence()
                .SetUpdate(true)
                .Append(DOTween.To(SetIrisRadius, openRadius, 0f, coverDuration).SetEase(Ease.InQuad))
                .Join(pawImage.rectTransform.DOScale(1f, coverDuration).SetEase(Ease.OutBack))
                .AppendInterval(coveredHoldDuration)
                .AppendCallback(() => onCovered?.Invoke())
                .AppendCallback(() => onReveal?.Invoke())
                .Append(DOTween.To(SetIrisRadius, 0f, openRadius, revealDuration).SetEase(Ease.OutQuad))
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

        private void SetIrisRadius(float radius)
        {
            irisMaterial.SetFloat(RadiusProperty, radius);
        }
    }
}
