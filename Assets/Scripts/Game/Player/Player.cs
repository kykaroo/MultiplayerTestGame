using Network;
using Photon.Pun;
using UnityEngine;

namespace Game.Player
{
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(PlayerHealth))]
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(PlayerController))]
    public class Player : MonoBehaviour
    {
        private PhotonView photonView;
        public PlayerHealth Health;
        public PlayerInput Input;
        public PlayerController Controller;
        private PlayerManager _playerManager;

        private void Start()
        {
            photonView = GetComponent<PhotonView>();
            _playerManager = PhotonView.Find((int)photonView.InstantiationData[0]).GetComponent<PlayerManager>();
            Health.OnDeath += Die;
            Controller.OnFallOffMap += () => Health.TakeDamage(float.MaxValue, default);
        }

        private void Die(Vector3 position)
        {
            Input._playerBody.constraints = RigidbodyConstraints.None;
            _playerManager.AddDeathToCounter();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            photonView.RPC(nameof(Controller.RPC_DisableHands), RpcTarget.All);
            photonView.RPC(nameof(RPC_DisablePlayer), RpcTarget.All);
        }

        [PunRPC]
        void RPC_DisablePlayer()
        {
            Input.enabled = false;
            Health.enabled = false;
            Controller.enabled = false;
            enabled = false;
        }
    }
}