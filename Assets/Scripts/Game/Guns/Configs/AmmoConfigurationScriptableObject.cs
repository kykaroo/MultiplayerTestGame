using UnityEngine;

namespace Game.Guns.Configs
{
    [CreateAssetMenu(fileName = "Ammo Config", menuName = "Guns/Ammo Config", order = 3)]
    public class AmmoConfigurationScriptableObject : ScriptableObject
    {
        [SerializeField] private bool discardClipOnReload;
        [SerializeField] private int maxAmmo;
        [SerializeField] private int clipSize;

        public bool DiscardClipOnReload => discardClipOnReload;

        public int MaxAmmo => maxAmmo;

        public int ClipSize => clipSize;
    }
}
