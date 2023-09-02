using Game.Guns.Handlers;

namespace Game.Guns.Modifiers
{
    public interface IModifier
    {
        void Apply(GunHandler gun);
    }
}