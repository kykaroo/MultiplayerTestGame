using Photon.Pun;
using UnityEngine;

namespace Game.Player.Movement
{
    public class PlayerSurfaceCheck : MonoBehaviour
    {
        [SerializeField] private GameObject head;
        [SerializeField] private SphereCollider headCollider;
        [SerializeField] private GameObject legs;
        [SerializeField] private SphereCollider legsCollider;
        [SerializeField] private LayerMask groundLayerMask;
        [SerializeField] private PhotonView photonView;

        private bool onSlopeTouch;
        private RaycastHit slopeHit;
        private float playerHeight;
        private Vector3 slopeMoveDirection;

        public bool Grounded { get; private set; }

        public float SurfaceAngle { get; private set; }

        public RaycastHit SlopeHit => slopeHit;


        private void Awake()
        {
            playerHeight = GetPlayerHeight();
        }
        
        private void Update()
        {
            if (!photonView.IsMine) return;
            
            Grounded = Physics.CheckSphere(legs.transform.position, legsCollider.radius + 0.001f, groundLayerMask);
            (SurfaceAngle, slopeHit) = CalculateSurfaceAngle();
        }

        private (float, RaycastHit) CalculateSurfaceAngle()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f, groundLayerMask))
            {
                return (Vector3.Angle(Vector3.up, slopeHit.normal), slopeHit);
            }

            return (0, slopeHit);
        }
        
        private float GetPlayerHeight()
        {
            float max = headCollider.radius;
            float min = legsCollider.radius;
            
            return Vector3.Distance(head.transform.position + new Vector3(0, max, 0),
                legs.transform.position + new Vector3(0, -min, 0));
        }
    }
}
