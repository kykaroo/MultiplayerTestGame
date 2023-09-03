using System.Collections.Generic;
using ExitGames.Client.Photon;
using Game.Guns.Configs;
using Game.Guns.Handlers;
using Photon.Pun;
using UnityEngine;

namespace Game.Player
{
    public class PlayerItemSelector : MonoBehaviourPunCallbacks
    {
        [SerializeField] private List<GunScriptableObject> allGuns;
        [SerializeField] private ItemHolder itemHolder;
        [SerializeField] private List<GunScriptableObject> defaultGuns;
        [SerializeField] private PhotonView PhotonView;
        
        public GunHandler ActiveGun;
        public List<GunHandler> AllPlayerGuns;
        public int currentListIndex;
        
        private void Awake()
        {
            AllPlayerGuns = new();
            currentListIndex = -1;
        }

        private void Start()
        {
            foreach (var gun in defaultGuns)
            {
                AllPlayerGuns.Add(new(gun, gun.PrefabPath, itemHolder));
            }
            
            if (!photonView.IsMine) return;

            EquipItem(0);
        }

        public void EquipItem(int index)
        {
            if (index == currentListIndex)
                return;

            if (currentListIndex != -1)
            {
                AllPlayerGuns[currentListIndex].ChangeState(false);
            }
            
            currentListIndex = index;
            
            AllPlayerGuns[currentListIndex].ChangeState(true);
            ActiveGun = AllPlayerGuns[currentListIndex];

            if (!PhotonView.IsMine) return;

            var hash = new Hashtable { { "itemIndex", currentListIndex } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }

        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps.ContainsKey("itemIndex") && !PhotonView.IsMine && Equals(targetPlayer, PhotonView.Owner))
            {
                EquipItem((int)changedProps["itemIndex"]);
            }
        }
    }
}
