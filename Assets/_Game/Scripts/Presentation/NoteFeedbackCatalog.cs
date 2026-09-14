using System;
using DuetCats.Content;
using UnityEngine;

namespace DuetCats.Presentation
{
    [CreateAssetMenu(
        fileName = "NoteFeedbackCatalog",
        menuName = "Duet Cats/Note Feedback Catalog")]
    public sealed class NoteFeedbackCatalog : ScriptableObject
    {
        [SerializeField] private SpecialNoteFeedback[] specialNotes = new SpecialNoteFeedback[0];

        public bool TryGetFeedback(NoteKind kind, out SpecialNoteFeedback feedback)
        {
            if (specialNotes == null)
            {
                feedback = null;
                return false;
            }

            for (var index = 0; index < specialNotes.Length; index++)
            {
                var candidate = specialNotes[index];
                if (candidate != null && candidate.Kind == kind)
                {
                    feedback = candidate;
                    return true;
                }
            }

            feedback = null;
            return false;
        }
    }

    [Serializable]
    public sealed class SpecialNoteFeedback
    {
        [SerializeField] private NoteKind kind = NoteKind.Rainbow;
        [SerializeField] private bool playRipple = true;
        [SerializeField] private Vector2 normalizedScreenPosition = new Vector2(0.5f, 0.5f);
        [SerializeField] private AudioClip hitSound;
        [SerializeField, Range(0f, 1f)] private float volume = 1f;

        public NoteKind Kind { get { return kind; } }
        public bool PlayRipple { get { return playRipple; } }
        public Vector2 NormalizedScreenPosition { get { return normalizedScreenPosition; } }
        public AudioClip HitSound { get { return hitSound; } }
        public float Volume { get { return volume; } }
    }
}
