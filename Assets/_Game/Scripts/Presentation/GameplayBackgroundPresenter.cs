using UnityEngine;

namespace DuetCats.Presentation
{
    [DefaultExecutionOrder(-100)]
    [DisallowMultipleComponent]
    public sealed class GameplayBackgroundPresenter : MonoBehaviour
    {
        [SerializeField] private Camera gameplayCamera;
        [SerializeField] private SpriteRenderer backgroundRenderer;
        [SerializeField] private Sprite portraitBackgroundSprite;
        [SerializeField] private Sprite landscapeBackgroundSprite;

        private int lastScreenWidth;
        private int lastScreenHeight;
        private float initialZ;

        private void Awake()
        {
            if (gameplayCamera == null)
            {
                gameplayCamera = Camera.main;
            }

            if (portraitBackgroundSprite == null && backgroundRenderer != null)
            {
                portraitBackgroundSprite = backgroundRenderer.sprite;
            }

            if (backgroundRenderer != null)
            {
                initialZ = backgroundRenderer.transform.position.z;
            }
        }

        private void Start()
        {
            RefreshLayout();
        }

        private void Update()
        {
            if (Screen.width == lastScreenWidth && Screen.height == lastScreenHeight)
            {
                return;
            }

            RefreshLayout();
        }

        private void RefreshLayout()
        {
            if (gameplayCamera == null || backgroundRenderer == null || portraitBackgroundSprite == null ||
                !gameplayCamera.orthographic)
            {
                Debug.LogError("GameplayBackgroundPresenter needs an orthographic camera, SpriteRenderer and portrait background.", this);
                enabled = false;
                return;
            }

            var isLandscape = Screen.width > Screen.height;
            backgroundRenderer.sprite = isLandscape && landscapeBackgroundSprite != null
                ? landscapeBackgroundSprite
                : portraitBackgroundSprite;

            var spriteSize = backgroundRenderer.sprite.bounds.size;
            var viewportHeight = gameplayCamera.orthographicSize * 2f;
            var viewportWidth = viewportHeight * gameplayCamera.aspect;
            var uniformScale = Mathf.Max(viewportWidth / spriteSize.x, viewportHeight / spriteSize.y);

            var backgroundTransform = backgroundRenderer.transform;
            backgroundTransform.position = new Vector3(
                gameplayCamera.transform.position.x,
                gameplayCamera.transform.position.y,
                initialZ);
            backgroundTransform.localScale = new Vector3(uniformScale, uniformScale, 1f);

            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
        }
    }
}
