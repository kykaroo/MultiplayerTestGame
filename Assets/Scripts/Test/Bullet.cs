using System.Collections;
using System.Collections.Generic;
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
        Destroy(this.GameObject());
        Destroy(bulletImpactObjects, 10f);
    }
}
