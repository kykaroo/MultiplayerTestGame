using System;
using System.Reflection;
using Game.Guns.Configs;
using UnityEngine;

namespace Game.Guns.Modifiers
{
    public class Vector3Modifier : AbstractValueModifier<Vector3>
    {
        public override void Apply(GunScriptableObject gun)
        {
            try
            {
                Vector3 value = GetAttribute<Vector3>(gun, out object targetObject, out FieldInfo field);
                value = new(value.x * Amount.x, value.y * Amount.y, value.z * Amount.z);
                field.SetValue(targetObject, value);
            }
            catch (InvalidPathSpecifiedException) { }
        }
    }
}