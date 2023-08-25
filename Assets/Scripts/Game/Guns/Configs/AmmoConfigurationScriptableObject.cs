using System;
using UnityEngine;

namespace Game.Guns
{
    [CreateAssetMenu(fileName = "Ammo Config", menuName = "Guns/Ammo Config", order = 3)]
    public class AmmoConfigurationScriptableObject : ScriptableObject, ICloneable
    {
        public bool discardClipOnReload;
    
        public int maxAmmo;
        public int clipSize;
    
        public int currentAmmo;
        public int currentClipAmmo;

        public void Reload()
        {
            if (discardClipOnReload)
            {
                int reloadAmount = Mathf.Min(clipSize, currentAmmo);
                currentClipAmmo = reloadAmount;
                currentAmmo -= reloadAmount;
            }
            else
            {
                int maxReloadAmount = Mathf.Min(clipSize, currentAmmo);
                int availableBulletInCurrentClip = clipSize - currentClipAmmo;
                int reloadAmount = Mathf.Min(maxReloadAmount, availableBulletInCurrentClip);

                currentClipAmmo += reloadAmount;
                currentAmmo -= reloadAmount;
            }
        }

        public bool CanReload()
        {
            return currentClipAmmo < clipSize && currentAmmo > 0;
        }

        public object Clone()
        {
            AmmoConfigurationScriptableObject config = CreateInstance<AmmoConfigurationScriptableObject>();
            Utilities.CopyValues(this, config);
            return config;
        }
    }
}
