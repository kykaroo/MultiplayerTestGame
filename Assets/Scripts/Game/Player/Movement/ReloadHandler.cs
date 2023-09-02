using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace Game.Player.Movement
{
    public class ReloadHandler : MonoBehaviour
    {
        [SerializeField] private PhotonView _photonView;
        [SerializeField] private bool autoReload = true;
        [SerializeField] private Animator handsAnimator;
        [SerializeField] private bool animationReload;
        [SerializeField] private CameraHolder cameraHolder;
        [Header("Needed only if animationReload = false")]
        [SerializeField] private float reloadTime;
        [SerializeField] private PlayerItemSelector itemSelector;
        
        private bool _isReloading;
        
        public event Action<int, int> OnAmmunitionUpdate;
        public event Action<float> OnReload;

        private void Awake()
        {
            cameraHolder.OnReloadEnded += EndReload;
            
            foreach (AnimationClip clip in handsAnimator.runtimeAnimatorController.animationClips)
            {
                switch (clip.name)
                {
                    case "Reloading":
                        reloadTime = clip.length;
                        break;
                }
            }
        }

        private void Update()
        {
            if(!_photonView.IsMine) return;
            
            itemSelector.activeGun.OnAmmunitionUpdate += AmmunitionUpdate; // TODO исправить костыль
        }

        public void TryToReload()
        {
            if (ShouldManualReload() || ShouldAutoReload())
            {
                _isReloading = true;
                OnReload?.Invoke(reloadTime);
                if (animationReload)
                {
                    cameraHolder.PlayAnimation();
                }
                else
                {
                    StartCoroutine(ReloadTimer());
                }
            }
        }
        private void EndReload()
        {
            if (!_photonView.IsMine) return;
            
            itemSelector.activeGun.EndReload();
            _isReloading = false;
            OnAmmunitionUpdate?.Invoke(itemSelector.activeGun.AmmoHandler.CurrentClipAmmo, itemSelector.activeGun.AmmoHandler.CurrentAmmo);
        }

        private IEnumerator ReloadTimer()
        {
            yield return new WaitForSeconds(reloadTime);
            EndReload();
        }
        
        private bool ShouldAutoReload()
        {
            return !_isReloading && autoReload &&  itemSelector.activeGun.AmmoHandler.CurrentClipAmmo == 0 && itemSelector.activeGun.CanReload();
        }

        private bool ShouldManualReload()
        {
            return !_isReloading && Input.GetKeyDown(KeyCode.R) && itemSelector.activeGun.CanReload();
        }
        
        private void AmmunitionUpdate(int currentAmmo, int maxAmmo)
        {
            OnAmmunitionUpdate?.Invoke(itemSelector.activeGun.AmmoHandler.CurrentClipAmmo, itemSelector.activeGun.AmmoHandler.CurrentAmmo);
        }
    }
}