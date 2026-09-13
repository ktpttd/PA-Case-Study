using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DuetCats.Presentation
{
    [DisallowMultipleComponent]
    public sealed class SongChoiceEndCardPresenter : MonoBehaviour
    {
        [Header("Artwork")]
        [FormerlySerializedAs("backgroundSprite")]
        [SerializeField] private Sprite portraitBackgroundSprite;
        [SerializeField] private Sprite landscapeBackgroundSprite;
        [SerializeField] private Sprite logoSprite;
        [SerializeField] private Sprite taglineSprite;
        [SerializeField] private Sprite leftSongCardSprite;
        [SerializeField] private Sprite rightSongCardSprite;
        [SerializeField] private Sprite handSprite;
        [SerializeField, Min(0.01f)] private float handMoveDuration = 0.75f;
        private GameObject endCardRoot;
        private RectTransform handTransform;
        private Tween handTween;
        private Tween revealTween;
        private Vector2 handLeftPosition;
        private Vector2 handRightPosition;

        public bool IsReady
        {
            get
            {
                return portraitBackgroundSprite != null && logoSprite != null && taglineSprite != null &&
                       leftSongCardSprite != null && rightSongCardSprite != null && handSprite != null;
            }
        }

        public void Show()
        {
            ShowForTransition(1f);
            StartHandMotion();
        }

        public void ShowForTransition(float initialScale)
        {
            if (!IsReady)
            {
                Debug.LogError("SongChoiceEndCardPresenter needs a portrait background and all artwork sprites.", this);
                return;
            }

            if (endCardRoot == null)
            {
                BuildLayout(Screen.width > Screen.height);
            }

            revealTween.Kill();
            KillHandMotion();
            endCardRoot.SetActive(true);
            endCardRoot.transform.localScale = Vector3.one * Mathf.Clamp(initialScale, 0.01f, 1f);
        }

        public void RevealFromTransition(float duration)
        {
            if (endCardRoot == null)
            {
                return;
            }

            revealTween.Kill();
            revealTween = endCardRoot.transform
                .DOScale(1f, duration)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    revealTween = null;
                    StartHandMotion();
                });
        }

        private void OnDestroy()
        {
            KillHandMotion();
            revealTween.Kill();
        }

        private void BuildLayout(bool isLandscape)
        {
            endCardRoot = new GameObject(
                "SongChoiceEndCard",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster));
            endCardRoot.transform.SetParent(transform, false);

            var canvas = endCardRoot.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;
            canvas.sortingOrder = 200;

            var scaler = endCardRoot.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = isLandscape
                ? new Vector2(1920f, 1080f)
                : new Vector2(1080f, 1920f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            var layout = isLandscape ? EndCardLayout.Landscape : EndCardLayout.Portrait;
            CreateBackground(isLandscape && landscapeBackgroundSprite != null
                ? landscapeBackgroundSprite
                : portraitBackgroundSprite);
            CreateImage("Logo", logoSprite, layout.logoPosition, layout.logoSize, false);
            CreateImage("Tagline", taglineSprite, layout.taglinePosition, layout.taglineSize, false);
            CreateImage("SongCardLeft", leftSongCardSprite, layout.leftCardPosition, layout.cardSize, false);
            CreateImage("SongCardRight", rightSongCardSprite, layout.rightCardPosition, layout.cardSize, false);
            var hand = CreateImage("Hand", handSprite, layout.handRightPosition, layout.handSize, false);
            handTransform = hand.rectTransform;
            handLeftPosition = layout.handLeftPosition;
            handRightPosition = layout.handRightPosition;

            endCardRoot.SetActive(false);
        }

        private void CreateBackground(Sprite backgroundSprite)
        {
            var background = CreateImage("Background", backgroundSprite, Vector2.zero, Vector2.zero, false);
            var rect = background.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            background.preserveAspect = false;
        }

        private Image CreateImage(
            string objectName,
            Sprite sprite,
            Vector2 anchoredPosition,
            Vector2 size,
            bool raycastTarget)
        {
            var child = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            child.transform.SetParent(endCardRoot.transform, false);

            var image = child.GetComponent<Image>();
            image.sprite = sprite;
            image.preserveAspect = true;
            image.raycastTarget = raycastTarget;

            var rect = image.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;
            return image;
        }

        private void StartHandMotion()
        {
            if (handTransform == null)
            {
                return;
            }

            KillHandMotion();
            handTransform.anchoredPosition = handLeftPosition;
            handTween = handTransform
                .DOAnchorPosX(handRightPosition.x, handMoveDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void KillHandMotion()
        {
            if (handTween == null)
            {
                return;
            }

            handTween.Kill();
            handTween = null;
        }

        private struct EndCardLayout
        {
            public static readonly EndCardLayout Portrait = new EndCardLayout(
                new Vector2(0f, 510f), new Vector2(900f, 302f),
                new Vector2(0f, 205f), new Vector2(780f, 252f),
                new Vector2(-235f, -250f), new Vector2(235f, -250f), new Vector2(445f, 585f),
                new Vector2(-235f, -400f), new Vector2(235f, -400f), new Vector2(210f, 210f));

            public static readonly EndCardLayout Landscape = new EndCardLayout(
                new Vector2(0f, 330f), new Vector2(620f, 208f),
                new Vector2(0f, 110f), new Vector2(530f, 171f),
                new Vector2(-210f, -235f), new Vector2(210f, -235f), new Vector2(330f, 433f),
                new Vector2(-205f, -345f), new Vector2(205f, -345f), new Vector2(165f, 165f));

            public readonly Vector2 logoPosition;
            public readonly Vector2 logoSize;
            public readonly Vector2 taglinePosition;
            public readonly Vector2 taglineSize;
            public readonly Vector2 leftCardPosition;
            public readonly Vector2 rightCardPosition;
            public readonly Vector2 cardSize;
            public readonly Vector2 handLeftPosition;
            public readonly Vector2 handRightPosition;
            public readonly Vector2 handSize;

            public EndCardLayout(
                Vector2 logoPosition,
                Vector2 logoSize,
                Vector2 taglinePosition,
                Vector2 taglineSize,
                Vector2 leftCardPosition,
                Vector2 rightCardPosition,
                Vector2 cardSize,
                Vector2 handLeftPosition,
                Vector2 handRightPosition,
                Vector2 handSize)
            {
                this.logoPosition = logoPosition;
                this.logoSize = logoSize;
                this.taglinePosition = taglinePosition;
                this.taglineSize = taglineSize;
                this.leftCardPosition = leftCardPosition;
                this.rightCardPosition = rightCardPosition;
                this.cardSize = cardSize;
                this.handLeftPosition = handLeftPosition;
                this.handRightPosition = handRightPosition;
                this.handSize = handSize;
            }
        }
    }
}
