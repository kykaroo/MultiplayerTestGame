using Game.ItemSystem.NewSystem;
using UnityEngine;

namespace Game.Player
{
    public class PlayerGroundCheck : MonoBehaviour
    {
        private PlayerInput _playerInput;

        private void Awake()
        {
            _playerInput = GetComponentInParent<PlayerInput>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject ==_playerInput.gameObject)
                return;
        
            _playerInput.SetGroundedState(true);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject ==_playerInput.gameObject)
                return;
        
            _playerInput.SetGroundedState(false);
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject ==_playerInput.gameObject)
                return;
        
            _playerInput.SetGroundedState(true);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject ==_playerInput.gameObject)
                return;
        
            _playerInput.SetGroundedState(true);
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject ==_playerInput.gameObject)
                return;
        
            _playerInput.SetGroundedState(false);
        }

        private void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject ==_playerInput.gameObject)
                return;
        
            _playerInput.SetGroundedState(true);
        }
    }
}
