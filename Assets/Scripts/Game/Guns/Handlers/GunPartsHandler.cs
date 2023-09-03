using Photon.Pun;
using UnityEngine;

namespace Game.Guns.Handlers
{
    public class GunPartsHandler : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particleSystem;
        [SerializeField] private PhotonView photonView;

        public ParticleSystem ParticleSystem => particleSystem;

        public void SetSelfActive(bool isActive)
        {
            photonView.RPC(nameof(RPC_SetSelfActive), RpcTarget.All, isActive);
        }

        [PunRPC]
        void RPC_SetSelfActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}