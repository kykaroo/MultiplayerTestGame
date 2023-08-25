using System;

namespace Game.Guns
{
    public abstract class CloneCreator
    {
        public static void CopyValues<T>(T @base, T copy)
        {
            Type type = @base.GetType();
            foreach (var field in type.GetFields())
            {
                field.SetValue(copy, field.GetValue(@base));
            }
        }
    }
}