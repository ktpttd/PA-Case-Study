using DG.Tweening;
using DuetCats.Gameplay;
using DuetCats.Session;
using UnityEngine;

namespace DuetCats.Presentation
{
    [DefaultExecutionOrder(150)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(GameSession))]
    public sealed class InstructionPresenter : MonoBehaviour
    {
        [SerializeField] private GameSession gameSession;
        [SerializeField] private NoteSystem noteSystem;
        [SerializeField] private CatView leftCat;
        [SerializeField] private CatView rightCat;
        [SerializeField] private GameObject instructionRoot;
        [SerializeField] private CanvasGroup instructionCanvasGroup;
        [SerializeField] private Transform leftInstruction;
        [SerializeField] private Transform rightInstruction;
        private bool wasVisible;
        private bool isStarting;
        private Vector3 initialLeftLocalPosition;
        private Vector3 initialRightLocalPosition;
        private Tween leftMovementTween;
        private Tween rightMovementTween;
        private Tween fadeTween;
        private Sequence introSequence;
        private bool isPlayingIntro;

        private void Awake()
        {
            ResolveInstructionReferences();
            CacheInitialPositions();
        }

        private void Start()
        {
            StartIntro();
        }

        private void Update()
        {
            var shouldBeVisible = !isPlayingIntro && gameSession != null && gameSession.Phase == GamePhase.Ready;
            if (shouldBeVisible != wasVisible)
            {
                SetVisible(shouldBeVisible);
            }
        }

        private void OnDestroy()
        {
            StopMovement();
            KillTween(ref fadeTween);
            introSequence.Kill();
            SetCatIntroOffsets(0f, 0f);
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
                .DOFade(0f, gameSession.GlobalSetting.Intro.FadeOutDuration)
                .SetUpdate(true)
                .OnComplete(StartGame);
        }

        private void StartGame()
        {
            if (instructionRoot != null)
            {
                instructionRoot.SetActive(false);
            }

            var introSongTime = noteSystem != null
                ? noteSystem.GetIntroSongTime()
                : gameSession.SongContent.FirstHitTime - gameSession.GlobalSetting.Gameplay.FallDuration;
            if (gameSession.TryStartRun(introSongTime))
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

        private void StartIntro()
        {
            if (gameSession == null || gameSession.Phase != GamePhase.Ready ||
                gameSession.GlobalSetting == null || noteSystem == null ||
                leftCat == null || rightCat == null)
            {
                RefreshVisibility();
                return;
            }

            isPlayingIntro = true;
            SetVisible(false);
            noteSystem.PrepareIntroNotes();

            var tuning = gameSession.GlobalSetting.Intro;
            SetCatIntroOffsets(-tuning.CatOutsideOffset, tuning.CatOutsideOffset);
            introSequence = DOTween.Sequence().SetUpdate(true);
            introSequence.AppendInterval(tuning.IntroDuration);
            introSequence.Insert(
                0f,
                DOVirtual.Float(-tuning.CatOutsideOffset, 0f, tuning.CatMoveDuration, leftCat.SetIntroOffsetX)
                    .SetEase(tuning.CatMoveEase));
            introSequence.Insert(
                0f,
                DOVirtual.Float(tuning.CatOutsideOffset, 0f, tuning.CatMoveDuration, rightCat.SetIntroOffsetX)
                    .SetEase(tuning.CatMoveEase));

            for (var index = 0; index < noteSystem.ActiveNotes.Count; index++)
            {
                var noteTransform = noteSystem.ActiveNotes[index].View.transform;
                var targetPosition = noteTransform.position;
                targetPosition.y = tuning.NoteReadyWorldY;
                var targetScale = noteTransform.localScale;
                introSequence.Insert(
                    tuning.CatMoveDuration,
                    noteTransform.DOMove(targetPosition, tuning.NoteMoveDuration).SetEase(tuning.NoteMoveEase));
                introSequence.Insert(
                    tuning.CatMoveDuration + tuning.NoteMoveDuration,
                    noteTransform.DOScale(
                            targetScale * tuning.NotePulseScale,
                            tuning.NotePulseDuration / (tuning.NotePulseCount * 2f))
                        .SetEase(tuning.NotePulseEase)
                        .SetLoops(tuning.NotePulseCount * 2, LoopType.Yoyo));
            }

            introSequence.InsertCallback(
                tuning.CatMoveDuration + tuning.NoteMoveDuration + tuning.InstructionShowDelay,
                ShowInstructionDuringIntro);
            introSequence.AppendCallback(CompleteIntro);
        }

        private void ShowInstructionDuringIntro()
        {
            if (instructionRoot == null)
            {
                return;
            }

            instructionRoot.SetActive(true);
            if (instructionCanvasGroup != null)
            {
                KillTween(ref fadeTween);
                instructionCanvasGroup.alpha = 0f;
                instructionCanvasGroup.interactable = false;
                instructionCanvasGroup.blocksRaycasts = false;
                fadeTween = instructionCanvasGroup
                    .DOFade(1f, gameSession.GlobalSetting.Intro.InstructionFadeInDuration)
                    .SetUpdate(true);
            }

            StartMovement();
        }

        private void CompleteIntro()
        {
            isPlayingIntro = false;
            introSequence = null;
            SetCatIntroOffsets(0f, 0f);
            wasVisible = true;
            isStarting = false;
            if (instructionCanvasGroup != null)
            {
                instructionCanvasGroup.interactable = true;
                instructionCanvasGroup.blocksRaycasts = true;
            }
        }

        private void SetCatIntroOffsets(float leftOffset, float rightOffset)
        {
            if (leftCat != null)
            {
                leftCat.SetIntroOffsetX(leftOffset);
            }

            if (rightCat != null)
            {
                rightCat.SetIntroOffsetX(rightOffset);
            }
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
            leftMovementTween = CreateMovementTween(
                leftInstruction,
                gameSession.GlobalSetting.Intro.CenterLocalXLeft);
            rightMovementTween = CreateMovementTween(
                rightInstruction,
                gameSession.GlobalSetting.Intro.CenterLocalXRight);
        }

        private Tween CreateMovementTween(Transform instructionTransform, float centerX)
        {
            return instructionTransform
                .DOLocalMoveX(centerX, gameSession.GlobalSetting.Intro.MoveDuration)
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
