using System.Collections;
using Game.Player.Movement;
using Game.Spawn;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Game.Player
{
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(PlayerHealth))]
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(PhotonTransformView))]
    public class Player : MonoBehaviourPunCallbacks
    {
        [SerializeField] private PhotonView photonView;
        public PlayerHealth Health;
        public PlayerInput Input;
        public PlayerController Controller;
        public ReloadHandler ReloadHandler;
        public float timeToDisableAfterDeath;
        public GameObject lobbyCamera;
        
        
        private PlayerManager _playerManager;

        private void Start()
        {
            if (!photonView.IsMine) return;

            photonView.RPC(nameof(RPC_DisablePlayerGameObject), RpcTarget.All);
            _playerManager = PhotonView.Find((int)photonView.InstantiationData[0]).GetComponent<PlayerManager>();
            Health.OnDeath += Die;
            Controller.OnFallOffMap += () => Health.TakeDamage(float.MaxValue, default);
        }

        private void Die(Vector3 position)
        {
            Input.playerBody.constraints = RigidbodyConstraints.None;
            _playerManager.AddDeathToCounter();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Input.Dead();
            Controller.DisableHands();
            photonView.RPC(nameof(RPC_DisablePlayer), RpcTarget.All);
            StartCoroutine(DisableCoroutine());
        }

        [PunRPC]
        void RPC_DisablePlayer()
        {
            Input.enabled = false;
            Health.isDead = true;
            Health.enabled = false;
            Controller.enabled = false;
            enabled = false;
        }

        public void Respawn()
        {
            Input.playerBody.constraints = RigidbodyConstraints.FreezeRotation;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Input.Respawn();
            Health.Respawn();
            Controller.EnableHands();
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.position = SpawnManager.Instance.GetSpawnPoint().position;
            photonView.RPC(nameof(RPC_EnablePlayer), RpcTarget.All);
            lobbyCamera.SetActive(false);
        }
        
        [PunRPC]
        void RPC_EnablePlayer()
        {
            Input.enabled = true;
            Health.isDead = false;
            Health.enabled = true;
            Controller.enabled = true;
            enabled = true;
            gameObject.SetActive(true);
        }

        private IEnumerator DisableCoroutine()
        {
            yield return new WaitForSeconds(timeToDisableAfterDeath);
            lobbyCamera.SetActive(true);
            photonView.RPC(nameof(RPC_DisablePlayerGameObject), RpcTarget.All);
        }

        [PunRPC]
        void RPC_DisablePlayerGameObject()
        {
            gameObject.SetActive(false);
        }
    }
}