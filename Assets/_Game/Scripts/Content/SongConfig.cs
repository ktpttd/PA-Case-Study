using System;
using UnityEngine;

namespace DuetCats.Content
{
    [CreateAssetMenu(fileName = "SongConfig", menuName = "Duet Cats/Song Config")]
    public sealed class SongConfig : ScriptableObject
    {
        [Header("Content")]
        [SerializeField] private TextAsset chart;
        [SerializeField] private AudioClip audioClip;

        [Header("Gameplay")]
        [SerializeField] private float chartOffset;
        [SerializeField, Min(1)] private int leftLaneCount = 3;

        [Header("Scoring")]
        [SerializeField] private VelocityScoreRule[] scoreRules = new VelocityScoreRule[0];
        [SerializeField] private SpecialNoteOverride[] specialNotes = new SpecialNoteOverride[0];

        public TextAsset Chart { get { return chart; } }
        public AudioClip AudioClip { get { return audioClip; } }
        public float ChartOffset { get { return chartOffset; } }
        public int LeftLaneCount { get { return leftLaneCount; } }
        public VelocityScoreRule[] ScoreRules { get { return scoreRules; } }
        public SpecialNoteOverride[] SpecialNotes { get { return specialNotes; } }
    }

    [Serializable]
    public sealed class VelocityScoreRule
    {
        [SerializeField] private int velocity;
        [SerializeField, Min(1)] private int points = 1;

        public int Velocity { get { return velocity; } }
        public int Points { get { return points; } }
    }

    [Serializable]
    public sealed class SpecialNoteOverride
    {
        [SerializeField, Min(1)] private int noteId;
        [SerializeField] private NoteKind kind;
        [SerializeField, Min(1)] private int points = 1;
        [SerializeField, Min(1)] private int comboNoteCount = 12;

        public int NoteId { get { return noteId; } }
        public NoteKind Kind { get { return kind; } }
        public int Points { get { return points; } }
        public int ComboNoteCount { get { return comboNoteCount; } }
    }
}
