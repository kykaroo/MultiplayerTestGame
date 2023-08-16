using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Bullet : Projectile<ItemInfo>
{
    private PhotonView _photonView;
    private Player _bulletOwner;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _bulletOwner = _photonView.Owner;
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
    
    private void CreateBulletImpact(Collision other)
    {
        ContactPoint contactPoint = other.contacts[0];
        Vector3 contactPos = contactPoint.point;

        GameObject bulletImpactObjects = 
            Instantiate(projectileImpactPrefab, contactPos + contactPoint.normal * 0.001f, 
                Quaternion.LookRotation(contactPoint.normal, Vector3.up) * projectileImpactPrefab.transform.rotation);
        bulletImpactObjects.transform.SetParent(other.transform);
            
        Destroy(bulletImpactObjects, 10f);
        
        if (_bulletOwner.IsLocal)
            PhotonNetwork.Destroy(gameObject);
    }
}
