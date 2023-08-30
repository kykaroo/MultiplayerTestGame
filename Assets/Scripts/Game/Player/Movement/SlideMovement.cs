using System;
using Photon.Pun;
using UnityEngine;

namespace Game.Player.Movement
{
    public class SlideMovement : MonoBehaviour
    {
        [SerializeField] private Rigidbody playerBody;
        [SerializeField] private Animator playerBodyAnimator;
        [SerializeField] private PlayerSurfaceCheck playerSurfaceCheck;
        [SerializeField] private PhotonView _photonView;
        [Header("Sliding")] 
        public float minimumSpeedToSlide;
        public float slideInitialForce;
        public float timeToGetVelocityBoostFromSlideMove;
        public float groundDrag;
        public float airDrag;
        
        private float _timerToGetVelocityBoostFromSlideMove;
        private bool _isSliding;
        private float _currentSpeed;
        private float _currentMovementMultiplier = 1f;
        private bool grounded;
        private float _actualSpeed;
        
        private static readonly int IsSliding = Animator.StringToHash("IsSliding");

        public event Action<bool> OnSlide;

        private void Awake()
        {
            playerSurfaceCheck.OnGrounded += b => grounded = b;
        }

        private void Update()
        {
            if (!_photonView.IsMine) return;

            _timerToGetVelocityBoostFromSlideMove -= Time.deltaTime;
            if (!_isSliding)
            {
                playerBody.drag = grounded ? groundDrag : airDrag;
            }
        }

        public void TryToStopSlide(float baseSpeed, float crouchSpeedMultiplier)
        {
            if ((!Input.GetKey(KeyCode.LeftControl) || _actualSpeed <= baseSpeed * crouchSpeedMultiplier) && _isSliding)
            {
                StopSlide();
            }
        }

        public void TryToSlide(Vector3 moveDirection)
        {
            if (grounded && Input.GetKey(KeyCode.LeftControl) && _actualSpeed >= minimumSpeedToSlide && !_isSliding)
            {
                StartSlide(moveDirection);
            }
        }
        
        private void StopSlide()
        {
            _isSliding = false;
            _photonView.RPC(nameof(RPC_PlaySlideAnimation), RpcTarget.All, _isSliding);
            OnSlide?.Invoke(_isSliding);
        }

        private void StartSlide(Vector3 moveDir)
        {
            _isSliding = true;
            playerBody.drag = 1f;
            _photonView.RPC(nameof(RPC_PlaySlideAnimation), RpcTarget.All, _isSliding);
            OnSlide?.Invoke(_isSliding);
            if (_timerToGetVelocityBoostFromSlideMove <= 0)
            {
                playerBody.AddForce(moveDir.normalized * (_actualSpeed * slideInitialForce), ForceMode.VelocityChange);
                _timerToGetVelocityBoostFromSlideMove = timeToGetVelocityBoostFromSlideMove;
            }
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
    }
}