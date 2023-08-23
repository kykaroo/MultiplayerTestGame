using Game.ItemSystem.NewSystem;
using Photon.Pun;
using UnityEngine;

namespace Game.Player
{
    public class ItemHolder : MonoBehaviour
    {
        private PhotonView _photonView;
        private GameObject gun;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
        }

        public GameObject CreateGun(string modelPrefabPath, Vector3 spawnPoint, Vector3 spawnRotation)
        {
            gun = PhotonNetwork.Instantiate(modelPrefabPath, Vector3.zero, Quaternion.identity, 1, new object[] {_photonView.ViewID});

            var ghjfg = gun.GetComponent<GunSpawner>()._photonView;

            _photonView.RPC(nameof(RPC_Spawn), RpcTarget.AllBuffered, spawnPoint, spawnRotation, gun.GetComponent<GunSpawner>()._photonView.ViewID);
            return gun;
        }
        
        [PunRPC]
        void RPC_Spawn(Vector3 spawnPoint, Vector3 spawnRotation, int photonId)
        {
            var gunPhotonView = PhotonView.Find(photonId);
            var gunTransform = gunPhotonView.transform;
            
            gunTransform.SetParent(gameObject.transform, false);
            gunTransform.localPosition = spawnPoint;
            gunTransform.localRotation = Quaternion.Euler(spawnRotation);
        }
    }
}
