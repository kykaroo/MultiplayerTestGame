using System;
using System.Reflection;
using Game.Guns.Handlers;

namespace Game.Guns.Modifiers
{
    public abstract class AbstractValueModifier<T> : IModifier
    {
        public string AttributeName;
        public T Amount;

        public abstract void Apply(GunHandler gun);

        protected FieldType GetAttribute<FieldType>(GunHandler gun, out object targetObject, out FieldInfo Field)
        {
            string[] paths = AttributeName.Split("/");
            string attribute = paths[paths.Length - 1];

            Type type = gun.GetType();
            object target = gun;

            for (int i = 0; i < paths.Length - 1; i++)
            {
                FieldInfo field = type.GetField(paths[i]);
                if (field == null)
                {
                    UnityEngine.Debug.LogError($"Невозможно применить модификатор к атрибуту {AttributeName}, потому что он не существует");
                    throw new InvalidPathSpecifiedException(AttributeName);
                }
                else
                {
                    target = field.GetValue(target);
                    type = target.GetType();
                }
            }

            FieldInfo attributeField = type.GetField(attribute);
            if (attributeField == null)
            {
                UnityEngine.Debug.LogError($"Невозможно применить модификатор к атрибуту {AttributeName}, потому что он не существует");
                throw new InvalidPathSpecifiedException(AttributeName);
            }

            Field = attributeField;
            targetObject = target;
            return (FieldType)attributeField.GetValue(target);
        }
    }
}