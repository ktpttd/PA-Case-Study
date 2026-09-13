using System.Collections.Generic;

namespace DuetCats.Content
{
    public enum NoteKind
    {
        Normal = 0,
        Rainbow = 1,
        Long = 2,
        Strong = 3
    }

    public sealed class RuntimeNote
    {
        internal RuntimeNote(int id, float hitTime, int laneIndex, NoteKind kind, int points)
        {
            Id = id;
            HitTime = hitTime;
            LaneIndex = laneIndex;
            Kind = kind;
            Points = points;
        }

        public int Id { get; private set; }
        public float HitTime { get; private set; }
        public int LaneIndex { get; private set; }
        public NoteKind Kind { get; private set; }
        public int Points { get; private set; }
    }

    public sealed class SongContent
    {
        private readonly RuntimeNote[] notes;

        internal SongContent(RuntimeNote[] notes, int laneCount, int leftLaneCount, int maxScore)
        {
            this.notes = notes;
            LaneCount = laneCount;
            LeftLaneCount = leftLaneCount;
            MaxScore = maxScore;
        }

        public IReadOnlyList<RuntimeNote> Notes { get { return notes; } }
        public int LaneCount { get; private set; }
        public int LeftLaneCount { get; private set; }
        public int RightLaneCount { get { return LaneCount - LeftLaneCount; } }
        public int MaxScore { get; private set; }
        public float FirstHitTime { get { return notes[0].HitTime; } }
        public float LastHitTime { get { return notes[notes.Length - 1].HitTime; } }
    }
}
