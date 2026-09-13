using DuetCats.Content;
using DuetCats.Presentation;

namespace DuetCats.Gameplay
{
    public sealed class ActiveNote
    {
        internal ActiveNote(RuntimeNote note, float spawnTime, NoteView view)
        {
            Note = note;
            SpawnTime = spawnTime;
            View = view;
        }

        public RuntimeNote Note { get; private set; }
        public float SpawnTime { get; private set; }
        public NoteView View { get; private set; }
    }
}
