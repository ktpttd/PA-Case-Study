using DuetCats.Session;
using UnityEngine;
using UnityEngine.UI;

namespace DuetCats.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(GameSession))]
    public sealed class SongProgressPresenter : MonoBehaviour
    {
        [SerializeField] private GameSession gameSession;
        [SerializeField] private Slider progressSlider;

        private float songDuration;

        private void Awake()
        {
            if (gameSession == null)
            {
                gameSession = GetComponent<GameSession>();
            }
        }

        private void Start()
        {
            if (gameSession == null || progressSlider == null || gameSession.SongConfig == null ||
                gameSession.SongConfig.AudioClip == null)
            {
                Debug.LogError("SongProgressPresenter needs GameSession, a Slider and an AudioClip.", this);
                enabled = false;
                return;
            }

            songDuration = gameSession.SongConfig.AudioClip.length;
            progressSlider.minValue = 0f;
            progressSlider.maxValue = 1f;
            progressSlider.wholeNumbers = false;
            progressSlider.interactable = false;
            progressSlider.direction = Slider.Direction.LeftToRight;
            RefreshProgress();
        }

        private void Update()
        {
            RefreshProgress();
        }

        private void RefreshProgress()
        {
            if (progressSlider == null || songDuration <= 0f)
            {
                return;
            }

            progressSlider.value = Mathf.Clamp01(gameSession.SongTime / songDuration);
        }
    }
}
