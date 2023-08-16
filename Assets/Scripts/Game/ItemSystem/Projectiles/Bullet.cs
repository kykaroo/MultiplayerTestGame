using Game.Player;
using Photon.Pun;
using UnityEngine;

namespace Game.ItemSystem
{
    public class Bullet : UsableItem<ProjectileConfig>
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
            
                other.gameObject.GetComponent<IDamagable>().TakeDamage(itemInfo.Damage);
            }
            else
            {
                CreateBulletImpact(other);   
            }
        }
    
        private void CreateBulletImpact(Collision other)
        {
            var contactPoint = other.contacts[0];
            var contactPos = contactPoint.point;

            var bulletImpactObjects = 
                Instantiate(itemInfo.ImpactPrefab, contactPos + contactPoint.normal * 0.001f, 
                    Quaternion.LookRotation(contactPoint.normal, Vector3.up) * itemInfo.ImpactPrefab.transform.rotation);
            bulletImpactObjects.transform.SetParent(other.transform);
            
            Destroy(bulletImpactObjects, 10f);
        
            if (_photonView.Owner.IsLocal)
                PhotonNetwork.Destroy(gameObject);
        }
    }
}
