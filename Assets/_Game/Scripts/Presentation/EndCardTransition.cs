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

        private bool hasOpenedEndCard;

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
        }

        private void Update()
        {
            if (hasOpenedEndCard || gameSession == null || gameSession.Phase != GamePhase.Ending)
            {
                return;
            }

            if (endCardController == null)
            {
                Debug.LogError("EndCardTransition needs an EndCardController in the scene.", this);
                enabled = false;
                return;
            }

            hasOpenedEndCard = true;
            var endCardCanvas = endCardController.GetComponentInParent<Canvas>();
            if (endCardCanvas != null)
            {
                endCardCanvas.overrideSorting = true;
                endCardCanvas.sortingOrder = 100;
            }

            endCardController.OpenEndCard();
            Luna.Unity.LifeCycle.GameEnded();
            gameSession.CompleteEnding();
        }
    }
}
