using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Gun : Item<GunInfo>
{
    public GameObject bullet;

    public float shootForce, upwardForce;
    public float timeBetweenShooting, spread, reloadTime, timeBetweenShoots;
    public int magazineSize, bulletPerTap;
    public bool allowButtonHold;

    internal int BulletsLeft, BulletsShot;
    internal bool Shooting, ReadyToShot, Reloading;
    public bool allowInvoke = true;

    public GameObject muzzleFlash;
    public TextMeshProUGUI ammunitionDisplay;
    public abstract override void Use();
    
    public abstract void Reload();
}
