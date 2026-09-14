using DuetCats.Controls;
using DuetCats.Session;
using UnityEngine;

namespace DuetCats.Gameplay
{
    [DefaultExecutionOrder(200)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(GameSession))]
    [RequireComponent(typeof(GameplayLayout))]
    [RequireComponent(typeof(NoteSystem))]
    [RequireComponent(typeof(CatInput))]
    public sealed class JudgementSystem : MonoBehaviour
    {
        [SerializeField] private GameSession gameSession;
        [SerializeField] private GameplayLayout gameplayLayout;
        [SerializeField] private NoteSystem noteSystem;
        [SerializeField] private CatInput catInput;

        private void Start()
        {
            if (gameSession == null || gameplayLayout == null || noteSystem == null ||
                catInput == null || gameSession.SongContent == null)
            {
                Debug.LogError("JudgementSystem needs valid gameplay dependencies and SongContent.", this);
                enabled = false;
            }
        }

        private void Update()
        {
            if (gameSession.Phase != GamePhase.Playing)
            {
                return;
            }

            var songTime = gameSession.SongTime;
            var config = gameSession.SongConfig;
            var activeNotes = noteSystem.ActiveNotes;

            for (var index = 0; index < activeNotes.Count;)
            {
                var activeNote = activeNotes[index];
                var timingOffset = songTime - activeNote.Note.HitTime;
                if (timingOffset < -config.HitTolerance)
                {
                    break;
                }

                if (timingOffset > config.HitTolerance)
                {
                    noteSystem.TryResolveMiss(activeNote);
                    return;
                }

                var catX = activeNote.Note.LaneIndex < gameSession.SongContent.LeftLaneCount
                    ? catInput.LeftCatX
                    : catInput.RightCatX;
                var noteX = gameplayLayout.GetLaneX(activeNote.Note.LaneIndex);
                var catchDistance = gameplayLayout.GetCatchDistance(
                    activeNote.Note.LaneIndex,
                    config.CatchDistance);
                if (Mathf.Abs(catX - noteX) <= catchDistance)
                {
                    noteSystem.TryResolveHit(activeNote);
                    continue;
                }

                index++;
            }

            if (gameSession.SongTime >= config.AudioClip.length &&
                noteSystem.HasSpawnedAllNotes && noteSystem.HasNoActiveNotes)
            {
                gameSession.TryFinish(GameOutcome.Win);
            }
        }
    }
}
