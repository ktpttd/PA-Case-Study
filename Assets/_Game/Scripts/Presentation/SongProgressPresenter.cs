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
        [SerializeField] private Image progressFillImage;

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
            if (gameSession == null || progressFillImage == null || gameSession.SongConfig == null ||
                gameSession.SongConfig.AudioClip == null)
            {
                Debug.LogError("SongProgressPresenter needs GameSession, a filled progress Image and an AudioClip.", this);
                enabled = false;
                return;
            }

            songDuration = gameSession.SongConfig.AudioClip.length;
            RefreshProgress();
        }

        private void Update()
        {
            RefreshProgress();
        }

        private void RefreshProgress()
        {
            if (progressFillImage == null || songDuration <= 0f)
            {
                return;
            }

            progressFillImage.fillAmount = Mathf.Clamp01(gameSession.SongTime / songDuration);
        }
    }
}
