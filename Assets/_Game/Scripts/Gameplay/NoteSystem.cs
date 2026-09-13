using System;
using System.Collections.Generic;
using DuetCats.Content;
using DuetCats.Presentation;
using DuetCats.Session;
using UnityEngine;

namespace DuetCats.Gameplay
{
    [DefaultExecutionOrder(100)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(GameSession))]
    [RequireComponent(typeof(GameplayLayout))]
    public sealed class NoteSystem : MonoBehaviour
    {
        [SerializeField] private GameSession gameSession;
        [SerializeField] private GameplayLayout gameplayLayout;
        [SerializeField] private NoteViewPool noteViewPool;

        private readonly List<ActiveNote> activeNotes = new List<ActiveNote>();
        private IReadOnlyList<RuntimeNote> notes;
        private int nextNoteIndex;
        private float fallDuration;
        private NoteView missedView;

        public IReadOnlyList<ActiveNote> ActiveNotes { get { return activeNotes; } }
        public event Action<RuntimeNote> NoteHit;
        public event Action<RuntimeNote> NoteMiss;
        public bool HasSpawnedAllNotes { get { return notes != null && nextNoteIndex >= notes.Count; } }
        public bool HasNoActiveNotes { get { return activeNotes.Count == 0; } }
        public int PrewarmedCapacity { get; private set; }

        private void Awake()
        {
            if (gameSession == null)
            {
                gameSession = GetComponent<GameSession>();
            }

            if (gameplayLayout == null)
            {
                gameplayLayout = GetComponent<GameplayLayout>();
            }
        }

        private void Start()
        {
            if (gameSession == null || gameplayLayout == null || noteViewPool == null ||
                gameSession.SongContent == null)
            {
                Debug.LogError("NoteSystem needs GameSession, GameplayLayout, NoteViewPool and valid SongContent.", this);
                enabled = false;
                return;
            }

            notes = gameSession.SongContent.Notes;
            fallDuration = gameSession.SongConfig.FallDuration;
            PrewarmedCapacity = CalculateMaximumConcurrentNotes(notes, fallDuration, gameSession.SongConfig.HitTolerance);
            noteViewPool.Prewarm(PrewarmedCapacity);
        }

        private void Update()
        {
            if (gameSession.Phase == GamePhase.Result)
            {
                ReleaseMissedView();
                return;
            }

            if (gameSession.Phase != GamePhase.Playing)
            {
                return;
            }

            SpawnDueNotes(gameSession.SongTime);
            UpdateVisualProgress(gameSession.SongTime);
        }

        public bool TryResolveHit(ActiveNote activeNote)
        {
            if (activeNote == null || !activeNotes.Remove(activeNote))
            {
                return false;
            }

            noteViewPool.Release(activeNote.View);
            var callback = NoteHit;
            if (callback != null)
            {
                callback(activeNote.Note);
            }

            return true;
        }

        public bool TryResolveMiss(ActiveNote activeNote)
        {
            if (activeNote == null || !activeNotes.Contains(activeNote) ||
                !gameSession.TryFinish(GameOutcome.Lose))
            {
                return false;
            }

            var missedNote = activeNote.Note;
            missedView = activeNote.View;
            noteViewPool.ShowBreak(missedView, GetSide(missedNote));
            activeNotes.Clear();
            noteViewPool.ReturnAllExcept(missedView);

            var callback = NoteMiss;
            if (callback != null)
            {
                callback(missedNote);
            }

            return true;
        }

        public void ReturnAll()
        {
            activeNotes.Clear();
            noteViewPool.ReturnAll();
            missedView = null;
        }

        private void ReleaseMissedView()
        {
            if (missedView == null)
            {
                return;
            }

            noteViewPool.Release(missedView);
            missedView = null;
        }

        private CatSide GetSide(RuntimeNote note)
        {
            return note.LaneIndex < gameSession.SongContent.LeftLaneCount
                ? CatSide.Left
                : CatSide.Right;
        }

        private void SpawnDueNotes(float songTime)
        {
            while (nextNoteIndex < notes.Count &&
                   notes[nextNoteIndex].HitTime - fallDuration <= songTime)
            {
                var note = notes[nextNoteIndex];
                var logicalX = gameplayLayout.GetLaneX(note.LaneIndex);
                var side = GetSide(note);
                var view = noteViewPool.Get(side, note.Kind);
                view.Show(note, logicalX, gameplayLayout);
                activeNotes.Add(new ActiveNote(note, note.HitTime - fallDuration, view));
                nextNoteIndex++;
            }
        }

        private void UpdateVisualProgress(float songTime)
        {
            for (var index = 0; index < activeNotes.Count; index++)
            {
                var activeNote = activeNotes[index];
                var duration = Mathf.Max(0.001f, activeNote.Note.HitTime - activeNote.SpawnTime);
                var progress = (songTime - activeNote.SpawnTime) / duration;
                activeNote.View.SetProgress(progress);
            }
        }

        private static int CalculateMaximumConcurrentNotes(
            IReadOnlyList<RuntimeNote> chartNotes,
            float noteFallDuration,
            float hitTolerance)
        {
            var releaseIndex = 0;
            var peak = 0;

            for (var index = 0; index < chartNotes.Count; index++)
            {
                var spawnTime = chartNotes[index].HitTime - noteFallDuration;
                while (releaseIndex < index &&
                       chartNotes[releaseIndex].HitTime + hitTolerance <= spawnTime)
                {
                    releaseIndex++;
                }

                peak = Mathf.Max(peak, index - releaseIndex + 1);
            }

            return peak;
        }
    }
}
