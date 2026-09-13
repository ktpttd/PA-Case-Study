using System.Collections.Generic;
using UnityEngine;

namespace DuetCats.Presentation
{
    [DisallowMultipleComponent]
    public sealed class NoteViewPool : MonoBehaviour
    {
        [SerializeField] private NoteView notePrefab;
        [SerializeField] private Transform container;

        private readonly Stack<NoteView> available = new Stack<NoteView>();
        private readonly HashSet<NoteView> leased = new HashSet<NoteView>();

        public bool IsConfigured { get { return notePrefab != null; } }
        public int AvailableCount { get { return available.Count; } }
        public int LeasedCount { get { return leased.Count; } }

        public void Prewarm(int capacity)
        {
            EnsureConfigured();

            var targetCount = Mathf.Max(0, capacity);
            while (available.Count + leased.Count < targetCount)
            {
                available.Push(CreateView());
            }
        }

        public NoteView Get()
        {
            EnsureConfigured();

            var view = available.Count > 0 ? available.Pop() : CreateView();
            leased.Add(view);
            view.gameObject.SetActive(true);
            return view;
        }

        public void Release(NoteView view)
        {
            if (view == null || !leased.Remove(view))
            {
                return;
            }

            view.ResetView();
            view.gameObject.SetActive(false);
            available.Push(view);
        }

        public void ReturnAll()
        {
            if (leased.Count == 0)
            {
                return;
            }

            var borrowedViews = new List<NoteView>(leased);
            for (var index = 0; index < borrowedViews.Count; index++)
            {
                Release(borrowedViews[index]);
            }
        }

        private NoteView CreateView()
        {
            var parent = container != null ? container : transform;
            var view = Instantiate(notePrefab, parent);
            view.gameObject.SetActive(false);
            return view;
        }

        private void EnsureConfigured()
        {
            if (!IsConfigured)
            {
                throw new System.InvalidOperationException("NoteViewPool needs a NoteView prefab.");
            }
        }
    }
}
