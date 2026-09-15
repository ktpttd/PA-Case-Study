using System;
using DG.Tweening;
using UnityEngine;

namespace DuetCats.Content
{
    [CreateAssetMenu(fileName = "GlobalSetting", menuName = "Duet Cats/Global Setting")]
    public sealed class GlobalSetting : ScriptableObject
    {
        [SerializeField] private GameplayTuning gameplay = new GameplayTuning();
        [SerializeField] private InputTuning input = new InputTuning();
        [SerializeField] private LayoutTuning layout = new LayoutTuning();
        [SerializeField] private IntroTuning intro = new IntroTuning();
        [SerializeField] private HitFeedbackTuning hitFeedback = new HitFeedbackTuning();
        [SerializeField] private ComboFeedbackTuning comboFeedback = new ComboFeedbackTuning();
        [SerializeField] private HudTuning hud = new HudTuning();
        [SerializeField] private ProgressTuning progress = new ProgressTuning();
        [SerializeField] private ScoreTuning score = new ScoreTuning();
        [SerializeField] private EndCardTuning endCard = new EndCardTuning();

        public GameplayTuning Gameplay { get { return gameplay; } }
        public InputTuning Input { get { return input; } }
        public LayoutTuning Layout { get { return layout; } }
        public IntroTuning Intro { get { return intro; } }
        public HitFeedbackTuning HitFeedback { get { return hitFeedback; } }
        public ComboFeedbackTuning ComboFeedback { get { return comboFeedback; } }
        public HudTuning Hud { get { return hud; } }
        public ProgressTuning Progress { get { return progress; } }
        public ScoreTuning Score { get { return score; } }
        public EndCardTuning EndCard { get { return endCard; } }
    }

    [Serializable]
    public sealed class GameplayTuning
    {
        [Min(0.01f)] public float FallDuration = 2.5f;
        [Min(0f)] public float HitTolerance = 0.1f;
        [Min(0.01f)] public float CatchDistance = 0.1f;
    }

    [Serializable]
    public sealed class InputTuning
    {
        [Min(0.01f)] public float DragSensitivity = 1f;
        [Min(0.01f)] public float KeyboardSpeed = 2f;
    }

    [Serializable]
    public sealed class LayoutTuning
    {
        public float JudgementWorldY = -0.5f;
        public float SpawnWorldY = 7f;
        public float LeftPlatformLogicalXOffset;
        public float RightPlatformLogicalXOffset;
        public float PlatformJudgementWorldYOffset = -0.45f;

        [Header("Portrait logical X ranges")]
        [Range(0f, 1f)] public float LeftMinX = 0.1f;
        [Range(0f, 1f)] public float LeftMaxX = 0.35f;
        [Range(0f, 1f)] public float RightMinX = 0.65f;
        [Range(0f, 1f)] public float RightMaxX = 0.9f;

        [Header("Landscape logical X ranges")]
        [Range(0f, 1f)] public float LandscapeLeftMinX = 0.3f;
        [Range(0f, 1f)] public float LandscapeLeftMaxX = 0.45f;
        [Range(0f, 1f)] public float LandscapeRightMinX = 0.55f;
        [Range(0f, 1f)] public float LandscapeRightMaxX = 0.7f;

        [Range(0.01f, 0.49f)] public float MaxCatchDistanceOfLaneSpacing = 0.45f;
    }

    [Serializable]
    public sealed class IntroTuning
    {
        public float CenterLocalXLeft = -100f;
        public float CenterLocalXRight = 100f;
        [Min(0.01f)] public float MoveDuration = 1f;
        [Min(0f)] public float FadeOutDuration = 0.25f;
        [Min(0.01f)] public float IntroDuration = 3f;
        [Min(0.01f)] public float CatMoveDuration = 1f;
        [Min(0f)] public float CatOutsideOffset = 6f;
        public Ease CatMoveEase = Ease.OutQuad;
        [Min(0.01f)] public float NoteMoveDuration = 0.5f;
        public float NoteReadyWorldY = 4f;
        public Ease NoteMoveEase = Ease.OutCubic;
        [Min(0.01f)] public float NotePulseDuration = 0.5f;
        [Min(1)] public int NotePulseCount = 2;
        [Range(0.01f, 1f)] public float NotePulseScale = 0.8f;
        public Ease NotePulseEase = Ease.InOutSine;
        [Min(0f)] public float InstructionShowDelay = 0.15f;
        [Min(0.01f)] public float InstructionFadeInDuration = 0.2f;
    }

    [Serializable]
    public sealed class HitFeedbackTuning
    {
        public string[] Messages = { "Sweet!", "Yummy!", "Taste!" };
        [Min(0.01f)] public float VisibleDuration = 0.55f;
        public float RiseDistance = 0.45f;

        public string GetRandomMessage()
        {
            return Messages == null || Messages.Length == 0
                ? string.Empty
                : Messages[UnityEngine.Random.Range(0, Messages.Length)];
        }
    }

    [Serializable]
    public sealed class ComboFeedbackTuning
    {
        [Min(0.01f)] public float VisibleDuration = 0.55f;
        [Min(0f)] public float PunchScaleStrength = 0.2f;
        [Min(0.01f)] public float PunchScaleDuration = 0.25f;
        public Vector2 ShakePositionStrength = new Vector2(5f, 8f);
        [Min(0.01f)] public float ShakePositionDuration = 0.25f;
        [Min(1)] public int ShakePositionVibrato = 10;
    }

    [Serializable]
    public sealed class HudTuning
    {
        [Range(0f, 1f)] public float VisibleAlpha = 1f;
        [Min(0.01f)] public float FadeDuration = 0.3f;
    }

    [Serializable]
    public sealed class ProgressTuning
    {
        [Min(0f)] public float MinFillWidth = 40f;
        [Min(1f)] public float OutsideOffset = 160f;
        [Min(0.01f)] public float TransitionDuration = 0.3f;
    }

    [Serializable]
    public sealed class ScoreTuning
    {
        [Min(1f)] public float OutsideOffset = 500f;
        [Min(0.01f)] public float TransitionDuration = 0.3f;
    }

    [Serializable]
    public sealed class EndCardTuning
    {
        [Min(0f)] public float ResultAnimationHoldDuration = 1f;
        [Min(0.01f)] public float HandMoveDuration = 0.75f;
        [Min(0.1f)] public float OpenRadius = 1.2f;
        [Range(0.001f, 0.1f)] public float IrisFeather = 0.015f;
        public Vector2 PortraitPawSize = new Vector2(300f, 300f);
        public Vector2 LandscapePawSize = new Vector2(200f, 200f);
        [Min(0.01f)] public float CoverDuration = 1f;
        [Min(0f)] public float CoveredHoldDuration = 0.5f;
        [Min(0.01f)] public float RevealDuration = 1f;
    }
}
