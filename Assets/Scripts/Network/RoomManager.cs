using System.IO;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    public class RoomManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Transform lobbyCameraAnchor;

        public Transform LobbyCameraAnchor => lobbyCameraAnchor;
        
        public static RoomManager Instance;
        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }

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
                PhotonNetwork.Instantiate("Game/PhotonPrefabs/PlayerManager", Vector3.zero, Quaternion.identity);
            }
        }
    }
}
