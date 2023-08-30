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
        [Header("Anti-force")]
        public float gravityScale;
        public float baseSpeed;
        [Tooltip("Умножается на скорость игрока для избежания огромных значений базовой скорости. Стандартно 10f.")]
        public float speedScale;
        public float sprintSpeedMultiplier;
        public float crouchSpeedMultiplier;
        public float midAirMoveSpeedMultiplier;

        private float _modifiedBaseSpeed;
        private int _slowStacks;
        private float _currentSpeed;
        private Vector3 _moveDirection;
        private const float Gravity = -9.81f;
        private float _currentMovementMultiplier;
        private Coroutine _slowCoroutine;
        private bool grounded = true;
        private bool _onSlope;
        private RaycastHit _slopeHit;
        private bool _isSliding;


        private void Awake()
        {
            _modifiedBaseSpeed = baseSpeed;
            playerSurfaceCheck.OnGrounded += b => grounded = b;
            slideMovement.OnSlide += b => _isSliding = b;
            playerSurfaceCheck.OnSlopeGround += SetOnSlopeState;
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
                    _currentSpeed = _modifiedBaseSpeed * sprintSpeedMultiplier;
                    break;
                case PlayerInput.MovementState.Crouching:
                    _currentSpeed = _modifiedBaseSpeed * crouchSpeedMultiplier;
                    break;
                case PlayerInput.MovementState.Walking:
                    _currentSpeed = _modifiedBaseSpeed;
                    break;
                case PlayerInput.MovementState.MidAirSprinting:
                    _currentSpeed = _modifiedBaseSpeed * midAirMoveSpeedMultiplier * sprintSpeedMultiplier;
                    break;
                case PlayerInput.MovementState.MidAirCrouch:
                    _currentSpeed = _modifiedBaseSpeed * midAirMoveSpeedMultiplier;
                    break;
                case PlayerInput.MovementState.MidAir:
                    _currentSpeed = _modifiedBaseSpeed * midAirMoveSpeedMultiplier;
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
            
            if (!grounded)
            {
                playerBody.AddForce(Vector3.up * (Gravity * gravityScale), ForceMode.Force);
            }

            if (grounded && _onSlope && !_isSliding)
            {
                playerBody.AddForce(GetSlopeMoveDirection(_moveDirection).normalized * (_currentSpeed * speedScale), ForceMode.Acceleration);    
            }

            if (grounded && !_onSlope && !_isSliding)
            {
                playerBody.AddForce(_moveDirection.normalized * (_currentSpeed * speedScale), ForceMode.Acceleration);
            }
            
            if (!grounded)
            {
                playerBody.AddForce(_moveDirection.normalized * (_currentSpeed * speedScale * midAirMoveSpeedMultiplier), ForceMode.Acceleration);   
            }
        }
        private Vector3 GetSlopeMoveDirection(Vector3 moveDir)
        {
            return Vector3.ProjectOnPlane(moveDir, _slopeHit.normal);
        }

        private void SetOnSlopeState(bool onSlope, RaycastHit slopeHit)
        {
            _onSlope = onSlope;
            _slopeHit = slopeHit;
        }
    }
}