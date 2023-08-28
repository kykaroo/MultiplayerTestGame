using UnityEngine;

namespace Game.Player
{
    public class PlayerSurfaceCheck : MonoBehaviour
    {
        [SerializeField] private GameObject head;
        [SerializeField] private SphereCollider headCollider;
        [SerializeField] private GameObject legs;
        [SerializeField] private SphereCollider legsCollider;
        [SerializeField] private LayerMask groundLayerMask;
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private float maxSlopeAngle;

        private bool onSlopeTouch;
        private RaycastHit slopeHit;
        private float playerHeight;
        private Vector3 slopeMoveDirection;

        private void Awake()
        {
            playerHeight = GetPlayerHeight();
        }
        
        private void Update()
        {
            _playerInput.SetGroundedState(Physics.CheckSphere(legs.transform.position, legsCollider.radius + 0.001f, groundLayerMask));
            _playerInput.SetOnSlopeState(OnSlope(), slopeHit);
        }

        private bool OnSlope()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f, groundLayerMask))
            {
                if (slopeHit.normal != Vector3.up)
                {
                    float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
                    return angle < maxSlopeAngle && angle != 0;
                }
            }
            
            return false;
        }
        
        private float GetPlayerHeight()
        {
            float max = headCollider.radius;
            float min = legsCollider.radius;
            
            return Vector3.Distance(head.transform.position + new Vector3(0, max, 0),
                legs.transform.position + new Vector3(0, -min, 0));;
        }
    }
}
