using UnityEngine;
using UnityEngine.EventSystems;

namespace DuetCats.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class InstructionStartTrigger : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private InstructionPresenter instructionPresenter;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (instructionPresenter != null)
            {
                instructionPresenter.TryStartFromInstruction();
            }
        }
    }
}
