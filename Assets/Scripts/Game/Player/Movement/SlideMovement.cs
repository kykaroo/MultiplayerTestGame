using Photon.Pun;
using UnityEngine;

namespace Game.Player.Movement
{
    public class SlideMovement : MonoBehaviour
    {
        [SerializeField] private Rigidbody playerBody;
        [SerializeField] private Animator playerBodyAnimator;
        [SerializeField] private PlayerSurfaceCheck playerSurfaceCheck;
        [SerializeField] private PhotonView photonView;
        [SerializeField] private BaseMovement baseMovement;
        [SerializeField] private float maxSlopeAngleToSlide;
        [Header("Sliding")] 
        public float minimumSpeedToSlide;
        public float slideInitialForce;
        public float timeToGetVelocityBoostFromSlideMove;
        public float groundDrag;
        public float airDrag;
        public AnimationCurve angleAcceleration;

        private float _timerToGetVelocityBoostFromSlideMove;
        private float _currentSpeed;
        private float _currentMovementMultiplier = 1f;
        private float _actualSpeed;

        public bool isSliding { get; private set; }

        private static readonly int IsSliding = Animator.StringToHash("IsSliding");
        private static readonly int SlideAngle = Animator.StringToHash("SlideAngle");

        private void Update()
        {
            if (!photonView.IsMine) return;

            _timerToGetVelocityBoostFromSlideMove -= Time.deltaTime;
            if (!isSliding)
            {
                playerBody.drag = playerSurfaceCheck.Grounded ? groundDrag : airDrag;
            }
            
            playerBodyAnimator.SetFloat(SlideAngle, GetSlideAngle());
        }

        private float GetSlideAngle()
        {
            var slideAngle = playerSurfaceCheck.SurfaceAngle;
            if (slideAngle < 10)
            {
                slideAngle = 10;
            }

            return slideAngle;
        }

        public void TryToStopSlide(float baseSpeed, float crouchSpeedMultiplier)
        {
            if ((!Input.GetKey(KeyCode.LeftControl) || _actualSpeed <= baseSpeed * crouchSpeedMultiplier) && isSliding)
            {
                StopSlide();
            }
        }

        public void TryToSlide()
        {
            if (playerSurfaceCheck.Grounded && Input.GetKey(KeyCode.LeftControl) &&
                _actualSpeed >= minimumSpeedToSlide && !isSliding && playerSurfaceCheck.SurfaceAngle > maxSlopeAngleToSlide)
            {
                StartSlide();
            }
        }
        
        private void StopSlide()
        {
            isSliding = false;
            photonView.RPC(nameof(RPC_PlaySlideAnimation), RpcTarget.All, isSliding);
        }

        private void StartSlide()
        {
            isSliding = true;
            playerBody.drag = 1f;
            photonView.RPC(nameof(RPC_PlaySlideAnimation), RpcTarget.All, isSliding);
            if (_timerToGetVelocityBoostFromSlideMove >= 0 || playerBody.velocity.y > 0) return;
            
            playerBody.AddForce(baseMovement.GetMoveDirection() * (_actualSpeed * slideInitialForce), ForceMode.VelocityChange);
            _timerToGetVelocityBoostFromSlideMove = timeToGetVelocityBoostFromSlideMove;
        }
        
        public void UpdateActualSpeed(float actualSpeed)
        {
            _actualSpeed = actualSpeed;
        }

        [PunRPC]
        void RPC_PlaySlideAnimation(bool isSliding)
        {
            playerBodyAnimator.SetBool(IsSliding, isSliding);
        }

        public float GetSlopeSlideAcceleration()
        {
            return angleAcceleration.Evaluate(playerSurfaceCheck.SurfaceAngle);
        }
    }
}