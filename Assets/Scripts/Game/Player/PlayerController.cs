using System;
using Photon.Pun;
using UnityEngine;

namespace Game.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Rigidbody playerBody;
        [SerializeField] private GameObject itemHolder;
        // [SerializeField] private GameObject nickNameCanvas;
        [SerializeField] private Transform cameraHolder;
        [SerializeField] private UnityEngine.Camera playerCameraPrefab;
        [SerializeField] private PhotonView photonView;
        [SerializeField] private LayerMask playerCollidersLayerMask;
        [SerializeField] private Transform rayOrigin;
        [SerializeField] private Transform player;
        
        private int _itemIndex;
        private int _previousItemIndex = -1;
        private float _currentReloadTime;
        private RaycastHit _hit;

        [HideInInspector]
        public UnityEngine.Camera playerCamera;

        public event Action OnFallOffMap;
        public event Action<string> OnPlayerOnCrosshair;

        private void Start()
        {
            if (photonView.IsMine)
            {
                // nickNameCanvas.SetActive(false);
                playerCamera = Instantiate(playerCameraPrefab, cameraHolder);
            }
            else
            {
                Destroy(playerBody);
            }
        }

        private void Update()
        {
            if (!photonView.IsMine) return;
            
            FallOffMapCheck();

            DisplayTargetedPlayerName();
        }

        private void DisplayTargetedPlayerName()
        {
            if (Physics.Raycast(rayOrigin.position, cameraHolder.forward, out _hit, 
                    100f, playerCollidersLayerMask))
            {
                if(!_hit.transform.root.TryGetComponent(out PhotonView hitPhotonView))
                {
                    OnPlayerOnCrosshair?.Invoke(string.Empty); 
                    return;
                }
                
                if (hitPhotonView == photonView)
                {
                    OnPlayerOnCrosshair?.Invoke(string.Empty);
                    return;
                }

                OnPlayerOnCrosshair?.Invoke(hitPhotonView.Owner.NickName);
            }
            else
            {
                OnPlayerOnCrosshair?.Invoke(string.Empty);
            }
        }

        private void FallOffMapCheck()
        {
            if (transform.position.y < -10f)
            {
                OnFallOffMap?.Invoke();
            }
        }

        [PunRPC]
        private void RPC_DisableHands()
        {
            itemHolder.SetActive(false);
            // nickNameCanvas.SetActive(false);
        }
        
        [PunRPC]
        private void RPC_EnableHands()
        {
            itemHolder.SetActive(true);
            // nickNameCanvas.SetActive(true);
        }

        public void DisableHands()
        {
            photonView.RPC(nameof(RPC_DisableHands), RpcTarget.All);
        }
        
        public void EnableHands()
        {
            photonView.RPC(nameof(RPC_EnableHands), RpcTarget.All);
            if (!photonView.IsMine) return;
            // nickNameCanvas.SetActive(false);
            cameraHolder.rotation = Quaternion.identity;
        }
    }
}

