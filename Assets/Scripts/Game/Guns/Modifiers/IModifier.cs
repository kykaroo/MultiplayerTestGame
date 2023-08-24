namespace Game.Guns.Modifiers
{
    public interface IModifier
    {
        void Apply(GunScriptableObject gun);
    }
}