using System.Reflection;
using Game.Guns.Handlers;
using UnityEngine;

namespace Game.Guns.Modifiers
{
    public class DamageModifier : AbstractValueModifier<float>
    {
        public override void Apply(GunHandler gun)
        {
            try
            {
                ParticleSystem.MinMaxCurve damageCurve =
                    GetAttribute<ParticleSystem.MinMaxCurve>(gun, out object targetObject, out FieldInfo field);

                switch (damageCurve.mode)
                {
                    case ParticleSystemCurveMode.TwoConstants:
                        damageCurve.constantMin *= Amount;
                        damageCurve.constantMax *= Amount;
                        break;
                    case ParticleSystemCurveMode.TwoCurves:
                        damageCurve.curveMultiplier *= Amount;
                        break;
                    case ParticleSystemCurveMode.Curve:
                        damageCurve.curveMultiplier *= Amount;
                        break;
                    case ParticleSystemCurveMode.Constant:
                        damageCurve.constant *= Amount;
                        break;
                }

                field.SetValue(targetObject, damageCurve);
            }
            catch (InvalidPathSpecifiedException) { }
        }
    }
}