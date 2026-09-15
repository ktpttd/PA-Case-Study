using DuetCats.Audio;
using DuetCats.Content;
using DuetCats.Gameplay;
using UnityEngine;

namespace DuetCats.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NoteSystem))]
    public sealed class SpecialNoteFeedbackPresenter : MonoBehaviour
    {
        [SerializeField] private NoteSystem noteSystem;
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
            if (noteSystem == null || config == null)
            {
                Debug.LogError(
                    "SpecialNoteFeedbackPresenter needs NoteSystem and its config.",
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
            if (note == null || !config.TryGetFeedback(note.Kind, out SpecialNoteFeedback feedback))
            {
                return;
            }

            if (feedback.PlayRipple)
            {
                // var normalizedPosition = feedback.NormalizedScreenPosition;
                // var screenPosition = new Vector2(
                //     Screen.width * Mathf.Clamp01(normalizedPosition.x),
                //     Screen.height * Mathf.Clamp01(normalizedPosition.y));
            }

            AudioManager.Play(feedback.AudioKey, feedback.Volume);
        }
    }
}
