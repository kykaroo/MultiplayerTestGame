using Game.Guns.Configs;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(FireRateAttribute))]
    public class FireRatePropertyDrawer : PropertyDrawer
    {
        public enum FireRateType
        {
            SecondsToAttack,
            AttacksPerSeconds
        }
    }
}