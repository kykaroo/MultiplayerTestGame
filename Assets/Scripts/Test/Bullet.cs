using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : Projectile
{
    private void OnCollisionEnter(Collision other)
    {
        ContactPoint contactPoint = other.contacts[0];
        Vector3 contactPos = contactPoint.point;
        
        GameObject bulletImpactObjects =
            Instantiate(projectileImpactPrefab, contactPos + contactPoint.normal * 0.001f, 
                Quaternion.LookRotation(contactPoint.normal, Vector3.up) * projectileImpactPrefab.transform.rotation);
        bulletImpactObjects.transform.SetParent(other.transform);
        other.collider.gameObject.GetComponent<IDamagable>()?.TakeDamage(damage);
        
        Destroy(bulletImpactObjects, 10f);
        GetComponent<PhotonView>().RPC(nameof(RPC_Destroy), RpcTarget.All);
    }

    [PunRPC]
    private void RPC_Destroy()
    {
        Destroy(gameObject);
    }
}
