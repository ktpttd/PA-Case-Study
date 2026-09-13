using UnityEngine;

namespace DuetCats.Presentation
{
    [CreateAssetMenu(fileName = "HitFeedbackConfig", menuName = "Duet Cats/Hit Feedback Config")]
    public sealed class HitFeedbackConfig : ScriptableObject
    {
        [SerializeField] private string[] messages = { "Sweet!", "Yummy!", "Taste!" };
        [SerializeField, Min(0.01f)] private float visibleDuration = 0.55f;

        public float VisibleDuration { get { return visibleDuration; } }

        public string GetRandomMessage()
        {
            if (messages == null || messages.Length == 0)
            {
                return string.Empty;
            }

            return messages[Random.Range(0, messages.Length)];
        }
    }
}
