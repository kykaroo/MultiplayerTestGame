using Photon.Pun;
using UnityEngine;

namespace Game.Player.PlayerInterfaces
{
    public interface ISlowable
    {
        public void Slow(float movementMultiplier, float slowTime, bool isStackable, int maxStacks, PhotonView targetPhotonView) { }
    }
}