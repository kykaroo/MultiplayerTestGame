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
        
        [HideInInspector] public GunHandler ActiveGun;
        [HideInInspector] public List<GunHandler> AllPlayerGuns;
        [HideInInspector] public int currentListIndex;
        
        private void Awake()
        {
            AllPlayerGuns = new();
            currentListIndex = -1;
        }

        private void Start()
        {
            if (!photonView.IsMine) return;
            
            foreach (var gun in defaultGuns)
            {
                AllPlayerGuns.Add(new(gun, gun.PrefabPath, itemHolder));
            }

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
            
            var hash = new Hashtable { { "itemIndex", currentListIndex } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }

        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps.ContainsKey("itemIndex") && !photonView.IsMine && Equals(targetPlayer, photonView.Owner))
            {
                EquipItem((int)changedProps["itemIndex"]);
            }
        }
    }
}
