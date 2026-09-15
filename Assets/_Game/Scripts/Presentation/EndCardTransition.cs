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
        [SerializeField] private EndGameTransitionPresenter endGameTransition;
        [SerializeField, Min(0f)] private float resultAnimationHoldDuration = 0.6f;

        private bool hasOpenedEndCard;
        private bool hasStartedTransition;
        private bool isWaitingForEndCard;
        private float endingStartedAt;

        private void Awake()
        {
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

            if (songChoiceEndCard == null || !songChoiceEndCard.IsReady ||
                endGameTransition == null || !endGameTransition.IsReady)
            {
                Debug.LogError("EndCardTransition needs configured SongChoiceEndCardPresenter and EndGameTransitionPresenter.", this);
                enabled = false;
                return;
            }

            if (endCardController == null)
            {
                Debug.LogError("EndCardTransition needs an EndCardController for its CTA button.", this);
                enabled = false;
                return;
            }

            if (hasStartedTransition)
            {
                return;
            }

            hasStartedTransition = true;
            var endCardCanvas = endCardController.GetComponentInParent<Canvas>();
            if (endCardCanvas != null)
            {
                endCardCanvas.overrideSorting = true;
                endCardCanvas.sortingOrder = 100;
                endCardCanvas.transform.localScale = Vector3.one;
            }

            endGameTransition.Play(
                () =>
                {
                    // The custom UI is visual-only; use the configured CTA button.
                    endCardController.OpenEndCard();
                    songChoiceEndCard.ShowForTransition(0.1f);
                },
                () => songChoiceEndCard.RevealFromTransition(endGameTransition.RevealDuration),
                CompleteEndGameTransition);
        }
        private void CompleteEndGameTransition()
        {
            hasOpenedEndCard = true;
            Luna.Unity.LifeCycle.GameEnded();
            gameSession.CompleteEnding();
        }

    }
}
