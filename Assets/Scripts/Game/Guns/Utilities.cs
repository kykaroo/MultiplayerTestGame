using System;

namespace Game.Guns
{
    public class Utilities
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