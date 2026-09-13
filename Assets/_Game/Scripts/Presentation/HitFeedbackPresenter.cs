using DuetCats.Content;
using DuetCats.Gameplay;
using DuetCats.Session;
using UnityEngine;

namespace DuetCats.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NoteSystem))]
    [RequireComponent(typeof(GameSession))]
    public sealed class HitFeedbackPresenter : MonoBehaviour
    {
        [SerializeField] private NoteSystem noteSystem;
        [SerializeField] private GameSession gameSession;
        [SerializeField] private HitFeedbackConfig config;
        [SerializeField] private HitFeedbackView leftFeedback;
        [SerializeField] private HitFeedbackView rightFeedback;

        private SongContent songContent;

        private void Awake()
        {
            if (noteSystem == null)
            {
                noteSystem = GetComponent<NoteSystem>();
            }

            if (gameSession == null)
            {
                gameSession = GetComponent<GameSession>();
            }
        }

        private void Start()
        {
            if (noteSystem == null || gameSession == null || config == null ||
                leftFeedback == null || rightFeedback == null || gameSession.SongContent == null)
            {
                Debug.LogError("HitFeedbackPresenter needs NoteSystem, config and one feedback view per cat.", this);
                enabled = false;
                return;
            }

            songContent = gameSession.SongContent;
            noteSystem.NoteHit += ShowFeedback;
        }

        private void OnDestroy()
        {
            if (noteSystem != null)
            {
                noteSystem.NoteHit -= ShowFeedback;
            }
        }

        private void ShowFeedback(RuntimeNote note)
        {
            var feedback = note.LaneIndex < songContent.LeftLaneCount ? leftFeedback : rightFeedback;
            feedback.Show(
                config.GetRandomMessage(),
                config.VisibleDuration,
                config.RiseDistance);
        }
    }
}
