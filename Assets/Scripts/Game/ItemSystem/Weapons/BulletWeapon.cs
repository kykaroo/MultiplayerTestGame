using System;
using Photon.Pun;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.ItemSystem.Weapon
{
    public class BulletWeapon : UsableItem<WeaponConfig>
    {
        public Camera fpsCam;
        public Transform attackPoint;
        private int _bulletsLeft;
        private int _bulletsShot;
        private bool _shooting;
        private bool _readyToShot;
        private bool _reloading;
        public bool allowInvoke = true;
        
        public event Action<float> OnReload;
        public event Action<int, int, int> OnAmmunitionChange;
        
        public override void M1ButtonAction()
        {
            _shooting = itemInfo.AllowButtonHold ? Input.GetKey(KeyCode.Mouse0) : Input.GetKeyDown(KeyCode.Mouse0);

            if (!_reloading && _shooting && _bulletsLeft <= 0) Reload();

            if (!_readyToShot || !_shooting || _reloading || _bulletsLeft <= 0) return;
            _bulletsShot = 0;

            Shoot();
        }

        public override void M2ButtonAction()
        {
            throw new System.NotImplementedException();
        }

        public override void RButtonAction()
        {
            if (_bulletsLeft < itemInfo.MagazineSize && !_reloading)
            {
                Reload();
            }
        }

        public override void OnItemChange()
        {
            OnAmmunitionChange?.Invoke(itemInfo.MagazineSize, _bulletsLeft, itemInfo.BulletPerTap);
        }

        private void Awake()
        {
            _bulletsLeft = itemInfo.MagazineSize;
            _readyToShot = true;
        }

        private void Shoot()
        {
            _readyToShot = false;

            var ray = fpsCam.ViewportPointToRay(new(0.5f, 0.5f, 0));

            var targetPoint = Physics.Raycast(ray, out var hit) ? hit.point : ray.GetPoint(75);
            var position = attackPoint.position;
            var directionWithoutSpread = targetPoint - position;

            var x = Random.Range(-itemInfo.Spread, itemInfo.Spread);
            var y = Random.Range(-itemInfo.Spread, itemInfo.Spread);

            var directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

            var vector3 = directionWithSpread.normalized * itemInfo.ShootForce +
                          fpsCam.transform.up * itemInfo.UpwardForce;

            var currentBullet = PhotonNetwork.Instantiate(itemInfo.ProjectileType.ProjectilePrefabPath, position, Quaternion.identity);

            currentBullet.transform.forward = directionWithSpread.normalized;

            currentBullet.GetComponent<Rigidbody>().AddForce(vector3, ForceMode.Impulse);


            if (itemInfo.MuzzleFlash != null)
            {
                Instantiate(itemInfo.MuzzleFlash, attackPoint.position, Quaternion.identity);
            }

            _bulletsLeft--;
            _bulletsShot++;

            if (allowInvoke)
            {
                Invoke(nameof(ResetShot), itemInfo.TimeBetweenShooting);

                allowInvoke = false;
            }

            if (_bulletsShot < itemInfo.BulletPerTap && _bulletsLeft > 0)
            {
                Invoke(nameof(Shoot), itemInfo.TimeBetweenShoots);
                OnAmmunitionChange?.Invoke(itemInfo.MagazineSize, _bulletsLeft, itemInfo.BulletPerTap);
            }
            
            OnAmmunitionChange?.Invoke(itemInfo.MagazineSize, _bulletsLeft, itemInfo.BulletPerTap);
        }

        private void ResetShot()
        {
            _readyToShot = true;
            allowInvoke = true;
        }

        private void Reload()
        {
            _reloading = true;
            
            OnReload?.Invoke(itemInfo.ReloadTime);
            
            Invoke(nameof(ReloadFinished), itemInfo.ReloadTime);
        }

        private void ReloadFinished()
        {
            _bulletsLeft = itemInfo.MagazineSize;
            OnAmmunitionChange?.Invoke(itemInfo.MagazineSize, _bulletsLeft, itemInfo.BulletPerTap);
            _reloading = false;
        }
    }
}