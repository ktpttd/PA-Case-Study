using DuetCats.Controls;
using DuetCats.Gameplay;
using Spine.Unity;
using UnityEngine;

namespace DuetCats.Presentation
{
    [DisallowMultipleComponent]
    public sealed class CatView : MonoBehaviour
    {
        [SerializeField] private CatSide side;
        [SerializeField] private CatInput catInput;
        [SerializeField] private GameplayLayout gameplayLayout;
        [SerializeField] private SkeletonAnimation skeletonAnimation;

        [Header("Spine Animations")]
        [SpineAnimation, SerializeField] private string idleAnimation = "Idle_Playing";
        [SpineAnimation, SerializeField] private string hitAnimation = "Eating_Single_Object";
        [SpineAnimation, SerializeField] private string winAnimation = "Cheering_Happy _Victory";
        [SpineAnimation, SerializeField] private string loseAnimation = "Miss_Object_Lose";

        public CatSide Side
        {
            get { return side; }
        }

        private void LateUpdate()
        {
            if (catInput == null || gameplayLayout == null || !gameplayLayout.HasWorldProjection)
            {
                return;
            }

            var logicalX = side == CatSide.Left ? catInput.LeftCatX : catInput.RightCatX;
            var worldPosition = transform.position;
            worldPosition.x = gameplayLayout.ToWorldX(logicalX, worldPosition);
            worldPosition.y = gameplayLayout.JudgementWorldY;
            transform.position = worldPosition;
        }

        public void PlayIdle()
        {
            PlayLoop(idleAnimation);
        }

        public void PlayHit()
        {
            if (!CanPlay(hitAnimation) || !CanPlay(idleAnimation))
            {
                return;
            }

            skeletonAnimation.AnimationState.SetAnimation(0, hitAnimation, false);
            skeletonAnimation.AnimationState.AddAnimation(0, idleAnimation, true, 0f);
        }

        public void PlayWin()
        {
            PlayLoop(winAnimation);
        }

        public void PlayLose()
        {
            PlayLoop(loseAnimation);
        }

        private void PlayLoop(string animationName)
        {
            if (!CanPlay(animationName))
            {
                return;
            }

            skeletonAnimation.AnimationState.SetAnimation(0, animationName, true);
        }

        private bool CanPlay(string animationName)
        {
            if (skeletonAnimation != null && skeletonAnimation.AnimationState != null &&
                !string.IsNullOrEmpty(animationName))
            {
                return true;
            }

            Debug.LogWarning("CatView needs a SkeletonAnimation and configured animation names.", this);
            return false;
        }
    }
}
