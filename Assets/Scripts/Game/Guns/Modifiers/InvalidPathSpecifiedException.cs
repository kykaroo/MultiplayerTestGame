using System;

namespace Game.Guns.Modifiers
{
    public class InvalidPathSpecifiedException : Exception
    {
        public InvalidPathSpecifiedException(string AttributeName): base($"{AttributeName} не существует в установленном пути.") { }
    }
}