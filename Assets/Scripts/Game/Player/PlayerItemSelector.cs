using System.Collections.Generic;
using Game.Guns;
using Game.Guns.Configs;
using Photon.Pun;
using UnityEngine;

namespace Game.Player
{
    public class PlayerItemSelector : MonoBehaviour
    {
        [SerializeField] private GunType Gun;
        [SerializeField] private Transform GunParent;
        [SerializeField] private List<GunScriptableObject> Guns;
        [SerializeField] private ItemHolder itemHolder;
        [SerializeField] private string modelPrefabPath;

        private PhotonView _photonView;

        [Space] [Header("Runtime Filled")] public GunScriptableObject ActiveGun;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
        }

        private void Start()
        {
            if (!_photonView.IsMine) return;
        
            GunScriptableObject gun = Guns.Find(gun => gun.Type == Gun);

            if (gun == null)
            {
                Debug.LogError($"No GunScriptableObject found fo GunType: {gun}");
                return;
            }

            ActiveGun = gun.Clone() as GunScriptableObject;
            
            ActiveGun.EntryPoint(modelPrefabPath, this, itemHolder);
        }
        
        // TODO Доделать смену оружия игроков

        /*void EquipItem(int index)
       {
           if (index == _previousItemIndex)
               return;

           _itemIndex = index;

           items[_itemIndex].itemGameObject.SetActive(true);

           if (_previousItemIndex != -1)
           {
               items[_previousItemIndex].itemGameObject.SetActive(false);
           }

           _previousItemIndex = _itemIndex;

           OnItemChange?.Invoke(items[_itemIndex]);

           items[_itemIndex].SetCamera(_camera);
           items[_itemIndex].OnItemChange();

           if (PV.IsMine)
           {
               var hash = new Hashtable();
               hash.Add("itemIndex", _itemIndex);
               PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
           }
       }*/

        /*public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps.ContainsKey("itemIndex") && !PV.IsMine && Equals(targetPlayer, PV.Owner))
            {
                EquipItem((int)changedProps["itemIndex"]);
            }
        }*/
        
        /*private void EquipItemCheck()
       {
           for (var i = 0; i < items.Length; i++)
           {
               if (Input.GetKeyDown((i + 1).ToString()))
               {
                   EquipItem(i);
                   break;
               }
           }

           if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
           {
               if (_itemIndex >= items.Length - 1)
               {
                   EquipItem(0);
               }
               else
               {
                   EquipItem(_itemIndex + 1);
               }
           }
           else
           {
               if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
               {
                   if (_itemIndex <= 0)
                   {
                       EquipItem(items.Length - 1);
                   }
                   else
                   {
                       EquipItem(_itemIndex - 1);
                   }
               }
           }
       }*/
    }
}
