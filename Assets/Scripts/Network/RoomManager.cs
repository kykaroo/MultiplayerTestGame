using System.IO;
using Game;
using Game.Camera;
using Game.Player;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = System.Object;

namespace Network
{
    public class RoomManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Transform lobbyCameraAnchor;
        [SerializeField] private EntryPoint _entryPoint;
        [SerializeField] private GuiFactory guiFactory;
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private PlayerController playerController;

        private PlayerManager _playerManager;

        public Transform LobbyCameraAnchor => lobbyCameraAnchor;
        
        public override void OnEnable()
        {
            base.OnEnable();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    
        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.buildIndex == 2)
            {
                _playerManager = PhotonNetwork.Instantiate("Game/PhotonPrefabs/PlayerManager", Vector3.zero, Quaternion.identity).GetComponent<PlayerManager>();
            }
        }
    }
}
