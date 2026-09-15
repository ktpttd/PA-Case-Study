using DuetCats.Audio;
using DuetCats.Content;
using DuetCats.Gameplay;
using DuetCats.Session;
using UnityEngine;

namespace DuetCats.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(GameSession))]
    [RequireComponent(typeof(NoteSystem))]
    public sealed class CatAnimationPresenter : MonoBehaviour
    {
        [SerializeField] private GameSession gameSession;
        [SerializeField] private NoteSystem noteSystem;
        [SerializeField] private CatView leftCat;
        [SerializeField] private CatView rightCat;

        private SongContent songContent;

        private void Start()
        {
            if (gameSession == null || noteSystem == null || leftCat == null || rightCat == null ||
                gameSession.SongContent == null)
            {
                Debug.LogError(
                    "CatAnimationPresenter needs GameSession, NoteSystem and both CatViews.", this);
                enabled = false;
                return;
            }

            songContent = gameSession.SongContent;
            gameSession.Finished += PlayOutcome;
            noteSystem.NoteHit += PlayHit;
            PlayIdle();
        }

        private void OnDestroy()
        {
            if (gameSession != null)
            {
                gameSession.Finished -= PlayOutcome;
            }

            if (noteSystem != null)
            {
                noteSystem.NoteHit -= PlayHit;
            }
        }

        private void PlayIdle()
        {
            leftCat.PlayIdle();
            rightCat.PlayIdle();
        }

        private void PlayHit(RuntimeNote note)
        {
            var cat = note.LaneIndex < songContent.LeftLaneCount ? leftCat : rightCat;
            cat.PlayHit();
        }

        private void PlayOutcome(GameOutcome outcome)
        {
            if (outcome == GameOutcome.Win)
            {
                AudioManager.Play(AudioKey.SFX_Win);
                leftCat.PlayWin();
                rightCat.PlayWin();
            }
            else if (outcome == GameOutcome.Lose)
            {
                AudioManager.Play(AudioKey.SFX_Lose);
                leftCat.PlayLose();
                rightCat.PlayLose();
            }
        }
    }
}
