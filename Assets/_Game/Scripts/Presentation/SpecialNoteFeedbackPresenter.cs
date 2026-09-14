using DuetCats.Content;
using DuetCats.Gameplay;
using UnityEngine;

namespace DuetCats.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NoteSystem))]
    [RequireComponent(typeof(AudioSource))]
    public sealed class SpecialNoteFeedbackPresenter : MonoBehaviour
    {
        [SerializeField] private NoteSystem noteSystem;
        [SerializeField] private AudioSource soundEffectSource;
        [SerializeField] private NoteFeedbackCatalog config;

        private void OnEnable()
        {
            if (noteSystem != null)
            {
                noteSystem.NoteHit += PlayFeedback;
            }
        }

        private void Start()
        {
            if (noteSystem == null || soundEffectSource == null || config == null)
            {
                Debug.LogError(
                    "SpecialNoteFeedbackPresenter needs NoteSystem, RippleEffect, an AudioSource and its config.",
                    this);
                enabled = false;
            }
        }

        private void OnDisable()
        {
            if (noteSystem != null)
            {
                noteSystem.NoteHit -= PlayFeedback;
            }
        }

        private void PlayFeedback(RuntimeNote note)
        {
            SpecialNoteFeedback feedback;
            if (note == null || !config.TryGetFeedback(note.Kind, out feedback))
            {
                return;
            }

            if (feedback.PlayRipple)
            {
                var normalizedPosition = feedback.NormalizedScreenPosition;
                var screenPosition = new Vector2(
                    Screen.width * Mathf.Clamp01(normalizedPosition.x),
                    Screen.height * Mathf.Clamp01(normalizedPosition.y));
            }

            if (feedback.HitSound != null)
            {
                soundEffectSource.PlayOneShot(feedback.HitSound, feedback.Volume);
            }
        }
    }
}