using System;
using DuetCats.Content;
using DuetCats.Gameplay;
using UnityEngine;

namespace DuetCats.Presentation
{
    [CreateAssetMenu(fileName = "NotePresentationConfig", menuName = "Duet Cats/Note Presentation Config")]
    public sealed class NotePresentationConfig : ScriptableObject
    {
        [SerializeField] private SideNotePresentation left;
        [SerializeField] private SideNotePresentation right;
        [SerializeField] private Sprite rainbowSprite;

        public Sprite GetSprite(CatSide side, NoteKind kind)
        {
            var sprite = kind == NoteKind.Rainbow
                ? rainbowSprite
                : GetSidePresentation(side).GetSprite(kind);
            if (sprite == null)
            {
                throw new InvalidOperationException(
                    "NotePresentationConfig is missing a sprite for " + side + " / " + kind + ".");
            }

            return sprite;
        }

        public Sprite GetBreakSprite(CatSide side)
        {
            var sprite = GetSidePresentation(side).BreakSprite;
            if (sprite == null)
            {
                throw new InvalidOperationException(
                    "NotePresentationConfig is missing a break sprite for " + side + ".");
            }

            return sprite;
        }

        private SideNotePresentation GetSidePresentation(CatSide side)
        {
            var presentation = side == CatSide.Left ? left : right;
            if (presentation == null)
            {
                throw new InvalidOperationException("NotePresentationConfig is missing the " + side + " presentation.");
            }

            return presentation;
        }
    }

    [Serializable]
    public sealed class SideNotePresentation
    {
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite longSprite;
        [SerializeField] private Sprite strongSprite;
        [SerializeField] private Sprite breakSprite;

        public Sprite BreakSprite { get { return breakSprite; } }

        public Sprite GetSprite(NoteKind kind)
        {
            switch (kind)
            {
                case NoteKind.Normal:
                    return normalSprite;
                case NoteKind.Long:
                    return longSprite;
                case NoteKind.Strong:
                    return strongSprite;
                default:
                    return null;
            }
        }
    }
}
