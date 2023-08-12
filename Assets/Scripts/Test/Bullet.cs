using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : Projectile<ItemInfo>
{
    private PhotonView _photonView;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<PhotonView>() != null && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            CreateBulletImpact(other);
            
            other.gameObject.GetComponent<IDamagable>().TakeDamage(damage);
        }
        else
        {
            CreateBulletImpact(other);   
        }
    }

    [PunRPC]
    private void RPC_Destroy()
    {
        Destroy(gameObject);
    }

    private void CreateBulletImpact(Collision other)
    {
        ContactPoint contactPoint = other.contacts[0];
        Vector3 contactPos = contactPoint.point;

        GameObject bulletImpactObjects = 
            Instantiate(projectileImpactPrefab, contactPos + contactPoint.normal * 0.001f, 
                Quaternion.LookRotation(contactPoint.normal, Vector3.up) * projectileImpactPrefab.transform.rotation);
        bulletImpactObjects.transform.SetParent(other.transform);
            
        Destroy(bulletImpactObjects, 10f);
        _photonView.RPC(nameof(RPC_Destroy), RpcTarget.All);   
    }
}
