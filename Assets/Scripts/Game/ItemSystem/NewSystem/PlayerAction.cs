using System;
using Photon.Pun;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    [SerializeField] private PlayerGunSelector GunSelector;

    private PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (!PV.IsMine) return;
        
        if (GunSelector.ActiveGun != null)
        {
            GunSelector.ActiveGun.Tick(Input.GetMouseButton(0));
        }
    }
}
