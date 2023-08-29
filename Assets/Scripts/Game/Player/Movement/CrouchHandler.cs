using UnityEngine;

namespace Game.Player.Movement
{
    public class CrouchHandler : MonoBehaviour
    {
        [SerializeField] private Animator playerBodyAnimator;
        private static readonly int IsCrouching = Animator.StringToHash("IsCrouching");

        public void SetCrouchState(bool isCrouching)
        {
            playerBodyAnimator.SetBool(IsCrouching, isCrouching);
        }
    }
}