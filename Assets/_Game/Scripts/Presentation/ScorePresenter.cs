using DG.Tweening;
using TMPro;
using DuetCats.Gameplay;
using UnityEngine;

namespace DuetCats.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ScoreState))]
    public sealed class ScorePresenter : MonoBehaviour
    {
        [SerializeField] private ScoreState scoreState;
        [SerializeField] private TextMeshProUGUI scoreText;

        private void Start()
        {
            scoreState.ScoreChanged += UpdateScore;
            UpdateScore(scoreState.Score);
        }

        private void OnDestroy()
        {
            scoreState.ScoreChanged -= UpdateScore;
        }

        private void UpdateScore(int score)
        {
            scoreText.SetText($"{score}");
            scoreText.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.25f, 1, 1);
        }
    }
}