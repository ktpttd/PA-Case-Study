using DuetCats.Session;
using UnityEngine;

namespace DuetCats.Presentation
{
    [DefaultExecutionOrder(300)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(GameSession))]
    public sealed class EndCardTransition : MonoBehaviour
    {
        [SerializeField] private GameSession gameSession;
        [SerializeField] private EndCardController endCardController;
        [SerializeField] private SongChoiceEndCardPresenter songChoiceEndCard;
        [SerializeField, Min(0f)] private float resultAnimationHoldDuration = 0.6f;

        private bool hasOpenedEndCard;
        private bool isWaitingForEndCard;
        private float endingStartedAt;

        private void Awake()
        {
            if (gameSession == null)
            {
                gameSession = GetComponent<GameSession>();
            }

            if (endCardController == null)
            {
                endCardController = FindObjectOfType<EndCardController>();
            }

            if (songChoiceEndCard == null)
            {
                songChoiceEndCard = FindObjectOfType<SongChoiceEndCardPresenter>();
            }
        }

        private void Update()
        {
            if (hasOpenedEndCard || gameSession == null || gameSession.Phase != GamePhase.Ending)
            {
                return;
            }

            if (!isWaitingForEndCard)
            {
                isWaitingForEndCard = true;
                endingStartedAt = Time.unscaledTime;
            }

            if (Time.unscaledTime < endingStartedAt + resultAnimationHoldDuration)
            {
                return;
            }

            if (songChoiceEndCard == null || !songChoiceEndCard.IsReady)
            {
                Debug.LogError("EndCardTransition needs a configured SongChoiceEndCardPresenter.", this);
                enabled = false;
                return;
            }

            if (endCardController == null)
            {
                Debug.LogError("EndCardTransition needs an EndCardController for its Luna ScreenClickButton.", this);
                enabled = false;
                return;
            }

            var endCardCanvas = endCardController.GetComponentInParent<Canvas>();
            if (endCardCanvas != null)
            {
                endCardCanvas.overrideSorting = true;
                endCardCanvas.sortingOrder = 100;
            }

            // The custom UI is visual-only; Luna's ScreenClickButton below it receives every tap.
            endCardController.EnableScreenClickCTA();
            songChoiceEndCard.Show();

            hasOpenedEndCard = true;
            Luna.Unity.LifeCycle.GameEnded();
            gameSession.CompleteEnding();
        }
    }
}
