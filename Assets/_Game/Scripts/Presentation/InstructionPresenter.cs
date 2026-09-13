using DuetCats.Session;
using UnityEngine;

namespace DuetCats.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(GameSession))]
    public sealed class InstructionPresenter : MonoBehaviour
    {
        [SerializeField] private GameSession gameSession;
        [SerializeField] private GameObject instructionRoot;

        private bool wasVisible;

        private void Awake()
        {
            if (gameSession == null)
            {
                gameSession = GetComponent<GameSession>();
            }
        }

        private void Start()
        {
            RefreshVisibility();
        }

        private void Update()
        {
            var shouldBeVisible = gameSession != null && gameSession.Phase == GamePhase.Ready;
            if (shouldBeVisible != wasVisible)
            {
                SetVisible(shouldBeVisible);
            }
        }

        private void RefreshVisibility()
        {
            SetVisible(gameSession != null && gameSession.Phase == GamePhase.Ready);
        }

        private void SetVisible(bool isVisible)
        {
            wasVisible = isVisible;
            if (instructionRoot != null)
            {
                instructionRoot.SetActive(isVisible);
            }
        }
    }
}
