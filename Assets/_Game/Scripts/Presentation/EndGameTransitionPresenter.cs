using System;
using DG.Tweening;
using DuetCats.Content;
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
        [SerializeField] private GlobalSetting globalSetting;

        private Sequence transitionSequence;

        public bool IsReady
        {
            get
            {
                return globalSetting != null && transitionRoot != null && blackOverlay != null &&
                       pawImage != null && irisMaterial != null;
            }
        }

        public float RevealDuration { get { return globalSetting.EndCard.RevealDuration; } }

        public void Play(Action onCovered, Action onReveal, Action onComplete)
        {
            if (!IsReady)
            {
                Debug.LogError("EndGameTransitionPresenter needs Transition Root, Black Overlay, Paw Image and Iris Material.", this);
                return;
            }

            transitionSequence.Kill();
            var tuning = globalSetting.EndCard;
            transitionRoot.SetActive(true);
            blackOverlay.material = irisMaterial;
            irisMaterial.SetFloat(AspectProperty, (float)Screen.width / Screen.height);
            irisMaterial.SetFloat(FeatherProperty, tuning.IrisFeather);
            SetIrisRadius(tuning.OpenRadius);
            pawImage.color = Color.white;
            pawImage.rectTransform.sizeDelta = Screen.width > Screen.height
                ? tuning.LandscapePawSize
                : tuning.PortraitPawSize;
            pawImage.rectTransform.localScale = Vector3.one * 0.1f;

            transitionSequence = DOTween.Sequence()
                .SetUpdate(true)
                .Append(DOTween.To(SetIrisRadius, tuning.OpenRadius, 0f, tuning.CoverDuration).SetEase(Ease.InQuad))
                .Join(pawImage.rectTransform.DOScale(2f, tuning.CoverDuration).SetEase(Ease.OutBack))
                .AppendInterval(tuning.CoveredHoldDuration)
                .AppendCallback(() => onCovered?.Invoke())
                .AppendCallback(() => onReveal?.Invoke())
                .Append(DOTween.To(SetIrisRadius, 0f, tuning.OpenRadius, tuning.RevealDuration).SetEase(Ease.OutQuad))
                .Join(pawImage.rectTransform.DOScale(0.15f, tuning.RevealDuration).SetEase(Ease.InBack))
                .Join(pawImage.DOFade(0f, tuning.RevealDuration * 0.7f))
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
