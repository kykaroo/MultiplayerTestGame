using Game.Guns.Configs;

namespace Game.Guns.Modifiers
{
    public interface IModifier
    {
        void Apply(GunScriptableObject gun);
    }
}