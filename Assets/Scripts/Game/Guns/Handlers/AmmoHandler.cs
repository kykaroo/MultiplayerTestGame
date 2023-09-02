using Game.Guns.Configs;
using UnityEngine;

namespace Game.Guns.Handlers
{
    public class AmmoHandler
    {
        public bool DiscardClipOnReload;
    
        public int MaxAmmo;
        public int ClipSize;
    
        public int CurrentAmmo;
        public int CurrentClipAmmo;
        public AmmoHandler(AmmoConfigurationScriptableObject ammoConfig)
        {
            DiscardClipOnReload = ammoConfig.DiscardClipOnReload;
            MaxAmmo = ammoConfig.MaxAmmo;
            ClipSize = ammoConfig.ClipSize;
            CurrentAmmo = MaxAmmo;
            CurrentClipAmmo = ClipSize;
        }
        
        public void Reload()
        {
            if (DiscardClipOnReload)
            {
                int reloadAmount = Mathf.Min(ClipSize, CurrentAmmo);
                CurrentClipAmmo = reloadAmount;
                CurrentAmmo -= reloadAmount;
            }
            else
            {
                int maxReloadAmount = Mathf.Min(ClipSize, CurrentAmmo);
                int availableBulletInCurrentClip = ClipSize - CurrentClipAmmo;
                int reloadAmount = Mathf.Min(maxReloadAmount, availableBulletInCurrentClip);

                CurrentClipAmmo += reloadAmount;
                CurrentAmmo -= reloadAmount;
            }
        }

        public bool CanReload()
        {
            return CurrentClipAmmo < ClipSize && CurrentAmmo > 0;
        }
    }
}