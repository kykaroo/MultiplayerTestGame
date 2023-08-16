using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProjectileGun : Gun
{
    public PhotonView photonView;
    public Camera fpsCam;
    public Transform attackPoint;
    
    private void Awake()
    {
        BulletsLeft = magazineSize;
        ReadyToShot = true;
    }
    
    public override void Use()
    {
        MyInput();
    }

    private void Update()
    {
        if (ammunitionDisplay != null)
        {
            ammunitionDisplay.text = $"{BulletsLeft / bulletPerTap} / {magazineSize / bulletPerTap}";
        }
    }

    private void MyInput()
    {
        Shooting = allowButtonHold ? Input.GetKey(KeyCode.Mouse0) : Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && BulletsLeft < magazineSize && !Reloading) Reload();

        if (ReadyToShot && Shooting && !Reloading && BulletsLeft <= 0) Reload();

        if (!ReadyToShot || !Shooting || Reloading || BulletsLeft <= 0) return;
        BulletsShot = 0;

        Shoot();
    }

    private void Shoot()
    {
        ReadyToShot = false;

        Ray ray = fpsCam.ViewportPointToRay(new(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        targetPoint = Physics.Raycast(ray, out hit) ? hit.point : ray.GetPoint(75);
        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);
        
        Vector3 vector3 = directionWithSpread.normalized * shootForce + fpsCam.transform.up * upwardForce;
        
        GameObject currentBullet = PhotonNetwork.Instantiate("ProjectTileGun/Bullet", attackPoint.position, Quaternion.identity);
        
        currentBullet.transform.forward = directionWithSpread.normalized;

        currentBullet.GetComponent<Rigidbody>().AddForce(vector3, ForceMode.Impulse);
        
        
        if (muzzleFlash != null)
        {
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
        }
        
        BulletsLeft--;
        BulletsShot++;

        if (allowInvoke)
        {
            Invoke(nameof(ResetShot), timeBetweenShooting);

            allowInvoke = false;
        }

        if (BulletsShot < bulletPerTap && BulletsLeft > 0)
        {
            Invoke(nameof(Shoot), timeBetweenShoots);
        }
    }

    private void ResetShot()
    {
        ReadyToShot = true;
        allowInvoke = true;
    }

    public override void Reload()
    {
        Reloading = true;
        Invoke(nameof(ReloadFinished), reloadTime);
    }

    private void ReloadFinished()
    {
        BulletsLeft = magazineSize;
        Reloading = false;
    }
}
