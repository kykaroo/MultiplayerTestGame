using Photon.Pun;
using UnityEngine;

namespace Game.Player.Movement
{
    public class CrouchHandler : MonoBehaviour
    {
        [SerializeField] private Animator playerBodyAnimator;
        [SerializeField] private PhotonView _photonView;

        public bool isCrouching;
        
        private static readonly int IsCrouching = Animator.StringToHash("IsCrouching");

        public void SetCrouchState(bool isCrouching)
        {
            
            _photonView.RPC(nameof(RPC_PlayCrouchAnimation), RpcTarget.All, isCrouching);
        }

        [PunRPC]
        void RPC_PlayCrouchAnimation(bool isCrouching)
        {
            playerBodyAnimator.SetBool(IsCrouching, isCrouching);
        }
    }
}