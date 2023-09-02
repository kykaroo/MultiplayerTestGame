using System;
using System.Collections;
using Game.Player.PlayerInterfaces;
using Photon.Pun;
using UnityEngine;

namespace Game.Player.Movement
{
    public class BaseMovement : MonoBehaviour, ISlowable
    {
        [SerializeField] private PhotonView photonView;
        [SerializeField] private Rigidbody playerBody;
        [SerializeField] private PlayerSurfaceCheck playerSurfaceCheck;
        [SerializeField] private SlideMovement slideMovement;
        [SerializeField] private CrouchHandler crouchHandler;
        [Header("Anti-force")]
        public float gravityScale;
        public float baseSpeed;
        [Tooltip("Умножается на скорость игрока для избежания огромных значений базовой скорости. Стандартно 10f.")]
        public float speedScale;
        public float sprintSpeedMultiplier;
        public float crouchSpeedMultiplier;
        public float midAirMoveSpeedMultiplier;
        public float onSlopeXMoveMultiplier;

        private float _modifiedBaseSpeed;
        private int _slowStacks;
        private float _maxSpeed;
        private Vector3 _moveDirection;
        private const float Gravity = -9.81f;
        private float _currentMovementMultiplier;
        private Coroutine _slowCoroutine;
        private bool _isCrouching;


        private void Awake()
        {
            if (!photonView.IsMine) return;
            
            _modifiedBaseSpeed = baseSpeed;
        }

        public void TryToMove(Vector3 moveDirection)
        {
            _moveDirection = moveDirection;
        }

        public void SetSpeed(PlayerInput.MovementState state)
        {
            switch (state)
            {
                case PlayerInput.MovementState.Sprinting:
                    _maxSpeed = _modifiedBaseSpeed * sprintSpeedMultiplier;
                    break;
                case PlayerInput.MovementState.Crouching:
                    _maxSpeed = _modifiedBaseSpeed * crouchSpeedMultiplier;
                    break;
                case PlayerInput.MovementState.Walking:
                    _maxSpeed = _modifiedBaseSpeed;
                    break;
                case PlayerInput.MovementState.MidAir:
                    _maxSpeed = _modifiedBaseSpeed * sprintSpeedMultiplier * midAirMoveSpeedMultiplier;
                    break;
            }
        }

        public void Slow(float movementMultiplier, float slowTime, bool isStackable, int maxStacks, PhotonView targetPhotonView)
        {
            photonView.RPC(nameof(RPC_Slow), targetPhotonView.Owner, movementMultiplier, slowTime, isStackable);
        }

        private void CalculateSlowPercent(float movementMultiplier)
        {
            if (_slowStacks == 1) _currentMovementMultiplier =  movementMultiplier;
            else _currentMovementMultiplier +=  movementMultiplier / _slowStacks;
        }

        private IEnumerator SlowDown(float slowTime)
        {
            _modifiedBaseSpeed = baseSpeed * _currentMovementMultiplier;
            yield return new WaitForSeconds(slowTime);

            _slowStacks = 0;
            _currentMovementMultiplier = 1;
            _modifiedBaseSpeed = baseSpeed;
        }

        [PunRPC]
        void RPC_Slow(float movementMultiplier, float slowTime, bool isStackable)
        {
            _slowStacks++;

            if (isStackable) CalculateSlowPercent(movementMultiplier);
            else _currentMovementMultiplier = movementMultiplier;
            
            if (_slowCoroutine != null)
            {
                StopCoroutine(_slowCoroutine);
            }

            _slowCoroutine = StartCoroutine(SlowDown(slowTime));
        }

        private void FixedUpdate()
        {
            if (!photonView.IsMine)
                return;
            
            Vector3 gravityForce = Vector3.up * (Gravity * gravityScale * playerBody.mass);
            playerBody.AddForce(gravityForce, ForceMode.Force);
            

            if (playerSurfaceCheck.Grounded && !slideMovement.isSliding)
            {
                playerBody.AddForce(
                    GetMoveDirection().normalized * (_maxSpeed * speedScale), ForceMode.Acceleration);
            }

            if (slideMovement.isSliding && playerBody.velocity.y < -0.1f)
            {
                playerBody.AddForce(
                    playerBody.velocity.normalized *
                    (_maxSpeed * speedScale * slideMovement.GetSlopeSlideAcceleration()), ForceMode.Acceleration);
            }

            if (!playerSurfaceCheck.Grounded)
            {
                playerBody.AddForce(_moveDirection.normalized * (_maxSpeed * speedScale * midAirMoveSpeedMultiplier), ForceMode.Acceleration);
            }
        }
        public Vector3 GetMoveDirection()
        {
            return Vector3.ProjectOnPlane(_moveDirection, playerSurfaceCheck.SlopeHit.normal);
        }
    }
}