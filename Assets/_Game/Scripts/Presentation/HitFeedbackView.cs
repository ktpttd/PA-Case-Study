using UnityEngine;

namespace DuetCats.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMesh))]
    public sealed class HitFeedbackView : MonoBehaviour
    {
        [SerializeField] private TextMesh textMesh;

        private float hideAt;

        private void Awake()
        {
            if (textMesh == null)
            {
                textMesh = GetComponent<TextMesh>();
            }

            Hide();
        }

        private void Update()
        {
            if (Time.unscaledTime >= hideAt)
            {
                Hide();
            }
        }

        public void Show(string message, float duration)
        {
            if (textMesh == null)
            {
                textMesh = GetComponent<TextMesh>();
            }

            if (textMesh == null || string.IsNullOrEmpty(message))
            {
                return;
            }

            gameObject.SetActive(true);
            textMesh.text = message;
            hideAt = Time.unscaledTime + duration;
        }

        private void Hide()
        {
            if (textMesh != null)
            {
                textMesh.text = string.Empty;
            }
        }
    }
}
