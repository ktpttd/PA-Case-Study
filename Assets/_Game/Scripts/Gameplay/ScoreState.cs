using DuetCats.Content;
using UnityEngine;

namespace DuetCats.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NoteSystem))]
    public sealed class ScoreState : MonoBehaviour
    {
        [SerializeField] private NoteSystem noteSystem;

        public int Score { get; private set; }

        private void Awake()
        {
            if (noteSystem == null)
            {
                noteSystem = GetComponent<NoteSystem>();
            }
        }

        private void OnEnable()
        {
            if (noteSystem != null)
            {
                noteSystem.NoteHit += Add;
            }
        }

        private void OnDisable()
        {
            if (noteSystem != null)
            {
                noteSystem.NoteHit -= Add;
            }
        }

        private void Add(RuntimeNote note)
        {
            Score += note.Points;
        }
    }
}
