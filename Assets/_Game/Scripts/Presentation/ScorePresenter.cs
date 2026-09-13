using DuetCats.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace DuetCats.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ScoreState))]
    public sealed class ScorePresenter : MonoBehaviour
    {
        [SerializeField] private ScoreState scoreState;
        [SerializeField] private Text scoreText;

        private void Awake()
        {
            if (scoreState == null)
            {
                scoreState = GetComponent<ScoreState>();
            }
        }

        private void Start()
        {
            if (scoreState == null || scoreText == null)
            {
                Debug.LogError("ScorePresenter needs ScoreState and a UI Text target.", this);
                enabled = false;
                return;
            }

            scoreState.ScoreChanged += UpdateScore;
            UpdateScore(scoreState.Score);
        }

        private void OnDestroy()
        {
            if (scoreState != null)
            {
                scoreState.ScoreChanged -= UpdateScore;
            }
        }

        private void UpdateScore(int score)
        {
            scoreText.text = score.ToString();
        }
    }
}
