using DG.Tweening;
using DuetCats.Session;
using UnityEngine;

namespace DuetCats.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(GameSession))]
    public sealed class InstructionPresenter : MonoBehaviour
    {
        [SerializeField] private GameSession gameSession;
        [SerializeField] private GameObject instructionRoot;
        [SerializeField] private CanvasGroup instructionCanvasGroup;
        [SerializeField] private Transform leftInstruction;
        [SerializeField] private Transform rightInstruction;
        [SerializeField] private float centerLocalXLeft;
        [SerializeField] private float centerLocalXRight;
        [SerializeField, Min(0.01f)] private float moveDuration = 0.8f;
        [SerializeField, Min(0f)] private float fadeOutDuration = 0.25f;

        private bool wasVisible;
        private bool isStarting;
        private Vector3 initialLeftLocalPosition;
        private Vector3 initialRightLocalPosition;
        private Tween leftMovementTween;
        private Tween rightMovementTween;
        private Tween fadeTween;

        private void Awake()
        {
            if (gameSession == null)
            {
                gameSession = GetComponent<GameSession>();
            }

            ResolveInstructionReferences();
            CacheInitialPositions();
        }

        private void Start()
        {
            RefreshVisibility();
        }

        private void Update()
        {
            var shouldBeVisible = gameSession != null && gameSession.Phase == GamePhase.Ready;
            if (shouldBeVisible != wasVisible)
            {
                SetVisible(shouldBeVisible);
            }
        }

        private void OnDestroy()
        {
            StopMovement();
            KillTween(ref fadeTween);
        }

        public void TryStartFromInstruction()
        {
            if (isStarting || gameSession == null || gameSession.Phase != GamePhase.Ready)
            {
                return;
            }

            isStarting = true;
            StopMovement();
            if (instructionCanvasGroup == null)
            {
                StartGame();
                return;
            }

            instructionCanvasGroup.interactable = false;
            instructionCanvasGroup.blocksRaycasts = false;
            fadeTween = instructionCanvasGroup
                .DOFade(0f, fadeOutDuration)
                .SetUpdate(true)
                .OnComplete(StartGame);
        }

        private void StartGame()
        {
            if (instructionRoot != null)
            {
                instructionRoot.SetActive(false);
            }

            if (gameSession.TryStartRun())
            {
                return;
            }

            isStarting = false;
            SetVisible(true);
        }

        private void RefreshVisibility()
        {
            SetVisible(gameSession != null && gameSession.Phase == GamePhase.Ready);
        }

        private void SetVisible(bool isVisible)
        {
            wasVisible = isVisible;
            if (instructionRoot == null)
            {
                return;
            }

            instructionRoot.SetActive(isVisible);
            if (isVisible)
            {
                isStarting = false;
                RestoreCanvasGroup();
                StartMovement();
            }
            else
            {
                StopMovement();
            }
        }

        private void StartMovement()
        {
            StopMovement();
            if (leftInstruction == null || rightInstruction == null)
            {
                Debug.LogError("InstructionPresenter needs Left and Right instruction transforms.", this);
                return;
            }

            leftInstruction.localPosition = initialLeftLocalPosition;
            rightInstruction.localPosition = initialRightLocalPosition;
            leftMovementTween = CreateMovementTween(leftInstruction, centerLocalXLeft);
            rightMovementTween = CreateMovementTween(rightInstruction, centerLocalXRight);
        }

        private Tween CreateMovementTween(Transform instructionTransform, float centerX)
        {
            return instructionTransform
                .DOLocalMoveX(centerX, moveDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void StopMovement()
        {
            KillTween(ref leftMovementTween);
            KillTween(ref rightMovementTween);

            if (leftInstruction != null)
            {
                leftInstruction.localPosition = initialLeftLocalPosition;
            }

            if (rightInstruction != null)
            {
                rightInstruction.localPosition = initialRightLocalPosition;
            }
        }

        private void ResolveInstructionReferences()
        {
            if (instructionRoot == null)
            {
                return;
            }

            if (instructionCanvasGroup == null)
            {
                instructionCanvasGroup = instructionRoot.GetComponent<CanvasGroup>();
            }

            if (leftInstruction == null)
            {
                leftInstruction = instructionRoot.transform.Find("Left");
            }

            if (rightInstruction == null)
            {
                rightInstruction = instructionRoot.transform.Find("Right");
            }
        }

        private void CacheInitialPositions()
        {
            if (leftInstruction != null)
            {
                initialLeftLocalPosition = leftInstruction.localPosition;
            }

            if (rightInstruction != null)
            {
                initialRightLocalPosition = rightInstruction.localPosition;
            }
        }

        private void RestoreCanvasGroup()
        {
            if (instructionCanvasGroup == null)
            {
                return;
            }

            KillTween(ref fadeTween);
            instructionCanvasGroup.alpha = 1f;
            instructionCanvasGroup.interactable = true;
            instructionCanvasGroup.blocksRaycasts = true;
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