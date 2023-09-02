using System;
using UnityEngine;

namespace Game.Player.Movement
{
    public class JumpHandler : MonoBehaviour
    {
        [SerializeField] private Rigidbody playerBody;
        [SerializeField] private PlayerSurfaceCheck playerSurfaceCheck;
        [Header("Jumping")]
        public float jumpCooldown;
        public bool autoJump;
        public float jumpForce;
        
        private float _jumpCooldownTimer;

        private void Update()
        {
            _jumpCooldownTimer -= Time.deltaTime;
        }

        public void TryToJump()
        {
            if (autoJump)
            {
                if (Input.GetKey(KeyCode.Space) && playerSurfaceCheck.Grounded && _jumpCooldownTimer <= 0)
                {
                    var velocity = playerBody.velocity;
                    velocity = new(velocity.x, 0f, velocity.z);
                    playerBody.velocity = velocity;
                    playerBody.AddForce(playerSurfaceCheck.SlopeHit.normal * jumpForce, ForceMode.VelocityChange); 
                    _jumpCooldownTimer = jumpCooldown;
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Space) && playerSurfaceCheck.Grounded)
                {
                    var velocity = playerBody.velocity;
                    velocity = new(velocity.x, 0f, velocity.z);
                    playerBody.velocity = velocity;
                    playerBody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
                }
            }
        }
    }
}