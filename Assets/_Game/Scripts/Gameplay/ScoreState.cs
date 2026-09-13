using System;
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
        public event Action<int> ScoreChanged;

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
            var callback = ScoreChanged;
            if (callback != null)
            {
                callback(Score);
            }
        }
    }
}
