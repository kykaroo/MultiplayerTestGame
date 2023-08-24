using Photon.Pun;
using UnityEngine;

namespace Game.Guns
{
    public class GunSpawner : MonoBehaviour
    {
        public PhotonView _photonView;
        public ParticleSystem ShootSystem;
        private Vector3 _spawnPoint;
        private Vector3 _spawnRotation;
        private PhotonView _parentPhotonView;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            // _parentPhotonView = PhotonView.Find((int)_photonView.InstantiationData[0]).GetComponent<PhotonView>();
        }

        // public void Initialize(Vector3 spawnPoint, Vector3 spawnRotation)
        // {
        //     _spawnPoint = spawnPoint;
        //     _spawnRotation = spawnRotation;
        //     // _parentPhotonView.RPC(nameof(gameObject.RPC_Spawn), RpcTarget.All, _spawnPoint, _spawnRotation);
        // }
        //
        // [PunRPC]
        // public void RPC_Spawn()
        // {
        //     print("gunSpawner");
        // transform.SetParent(gunParentTransform, false);
        // transform.localPosition = spawnPoint;
        // transform.localRotation = Quaternion.Euler(spawnRotation);
        // }
    }
}