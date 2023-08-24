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
            gun = PhotonNetwork.Instantiate(modelPrefabPath, Vector3.zero, Quaternion.identity, 0, new object[] {_photonView.ViewID});

            var gunViewId = gun.GetComponent<PhotonView>().ViewID;

            _photonView.RPC(nameof(RPC_Spawn), RpcTarget.AllBuffered, spawnPoint, spawnRotation, gunViewId);
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
