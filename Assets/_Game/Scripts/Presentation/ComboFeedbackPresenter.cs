using System.Collections;
using DG.Tweening;
using DuetCats.Content;
using DuetCats.Gameplay;
using DuetCats.Session;
using TMPro;
using UnityEngine;

namespace DuetCats.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NoteSystem))]
    [RequireComponent(typeof(GameSession))]
    public sealed class ComboFeedbackPresenter : MonoBehaviour
    {
        [SerializeField] private NoteSystem noteSystem;
        [SerializeField] private GameSession gameSession;
        [SerializeField] private TextMeshProUGUI comboText;
        private int comboFeedbackIndex;
        private int comboNotesRemaining;
        private Coroutine hideRoutine;
        private RectTransform comboRect;
        private Vector2 initialAnchoredPosition;
        private Vector3 initialLocalScale;
        private Tween punchScaleTween;
        private Tween shakePositionTween;

        private void Start()
        {
            if (noteSystem == null || gameSession == null || comboText == null ||
                gameSession.SongContent == null || gameSession.GlobalSetting == null)
            {
                Debug.LogError("ComboFeedbackPresenter needs NoteSystem, GameSession and a combo TMP text.", this);
                enabled = false;
                return;
            }

            comboRect = comboText.rectTransform;
            initialAnchoredPosition = comboRect.anchoredPosition;
            initialLocalScale = comboRect.localScale;
            comboText.gameObject.SetActive(false);
            noteSystem.NoteHit += HandleNoteHit;
        }

        private void OnDestroy()
        {
            if (noteSystem != null)
            {
                noteSystem.NoteHit -= HandleNoteHit;
            }

            KillTween(ref punchScaleTween);
            KillTween(ref shakePositionTween);
        }

        private void HandleNoteHit(RuntimeNote note)
        {
            var specialNote = FindSpecialNote(note.Id);
            if (specialNote != null)
            {
                comboFeedbackIndex = 0;
                comboNotesRemaining = specialNote.ComboNoteCount;
                Hide();
                return;
            }

            if (comboNotesRemaining <= 0)
            {
                return;
            }

            comboText.SetText(GetComboMessage(comboFeedbackIndex++));
            comboText.gameObject.SetActive(true);
            PlayTextAnimation();
            comboNotesRemaining--;

            if (hideRoutine != null)
            {
                StopCoroutine(hideRoutine);
            }

            hideRoutine = StartCoroutine(HideAfterDelay());
        }

        private SpecialNoteOverride FindSpecialNote(int noteId)
        {
            var specialNotes = gameSession.SongConfig.SpecialNotes;
            if (specialNotes == null)
            {
                return null;
            }

            // ponytail: special-note lists are tiny; add a lookup only if that changes.
            for (var index = 0; index < specialNotes.Length; index++)
            {
                if (specialNotes[index].NoteId == noteId)
                {
                    return specialNotes[index];
                }
            }

            return null;
        }

        private IEnumerator HideAfterDelay()
        {
            yield return new WaitForSeconds(gameSession.GlobalSetting.ComboFeedback.VisibleDuration);
            hideRoutine = null;
            HideVisual();
        }

        private void Hide()
        {
            if (hideRoutine != null)
            {
                StopCoroutine(hideRoutine);
                hideRoutine = null;
            }

            HideVisual();
        }

        private void PlayTextAnimation()
        {
            KillTween(ref punchScaleTween);
            KillTween(ref shakePositionTween);
            comboRect.anchoredPosition = initialAnchoredPosition;
            comboRect.localScale = initialLocalScale;

            var tuning = gameSession.GlobalSetting.ComboFeedback;
            punchScaleTween = comboRect.DOPunchScale(
                Vector3.one * tuning.PunchScaleStrength,
                tuning.PunchScaleDuration);
            shakePositionTween = comboRect.DOShakeAnchorPos(
                tuning.ShakePositionDuration,
                tuning.ShakePositionStrength,
                tuning.ShakePositionVibrato);
        }

        private void HideVisual()
        {
            KillTween(ref punchScaleTween);
            KillTween(ref shakePositionTween);

            if (comboRect != null)
            {
                comboRect.anchoredPosition = initialAnchoredPosition;
                comboRect.localScale = initialLocalScale;
            }

            comboText.gameObject.SetActive(false);
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

        private static string GetComboMessage(int index)
        {
            if (index == 0)
            {
                return "Nice";
            }

            if (index == 1)
            {
                return "Great";
            }

            return "Perfect x" + (index - 1);
        }
    }
}
