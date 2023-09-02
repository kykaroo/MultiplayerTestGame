using System;
using System.Linq;
using Game.Guns.Configs;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Guns.Handlers
{
    public class ShootHandler
    {
        public bool IsHitScan;
        public string BulletPrefabPath;
        public float BulletSpawnForce;
        public float FireRate;
        public float RecoilRecoverySpeed;
        public float MaxSpreadTime;
        public BulletSpreadType SpreadType;
        public Vector3 Spread;
        public float SpreadMultiplier;
        public Texture2D SpreadTexture;
        public LayerMask HitMask;
        
        public ShootHandler(ShootConfigurationScriptableObject shootConfig)
        {
            IsHitScan = shootConfig.IsHitScan;
            BulletPrefabPath = shootConfig.BulletPrefabPath;
            BulletSpawnForce = shootConfig.BulletSpawnForce;
            FireRate = shootConfig.FireRate;
            RecoilRecoverySpeed = shootConfig.RecoilRecoverySpeed;
            MaxSpreadTime = shootConfig.MaxSpreadTime;
            SpreadType = shootConfig.SpreadType;
            Spread = shootConfig.Spread;
            SpreadMultiplier = shootConfig.SpreadMultiplier;
            SpreadTexture = shootConfig.SpreadTexture;
            HitMask = shootConfig.HitMask;
        }
        
         public Vector3 GetSpread(float shootTime = 0)
        {
            var spread = Vector3.zero;

            if (SpreadType == BulletSpreadType.Simple)
            {
                spread = Vector3.Lerp(Vector3.zero,
                    new(Random.Range(-Spread.x, Spread.x), Random.Range(-Spread.y, Spread.y),
                        Random.Range(-Spread.z, Spread.z)), Mathf.Clamp01(shootTime / MaxSpreadTime));
            }
            else
            {
                if (SpreadType == BulletSpreadType.TextureBased)
                {
                    spread = GetTextureDirection(shootTime);
                    spread *= SpreadMultiplier;
                }
            }
        
            return spread;
        }

        private Vector3 GetTextureDirection(float shootTime)
        {
            Vector2 halfSize = new(SpreadTexture.width / 2f, SpreadTexture.height / 2f);
            var halfSquareExtents = Mathf.CeilToInt(Mathf.Lerp(0.01f, halfSize.x, Mathf.Clamp01(shootTime / MaxSpreadTime)));
        
            var minX = Mathf.FloorToInt(halfSize.x) - halfSquareExtents;
            var minY = Mathf.FloorToInt(halfSize.y) - halfSquareExtents;

            var sampleColors = SpreadTexture.GetPixels(minX, minY, halfSquareExtents * 2, halfSquareExtents * 2);

            var coloursAsGrey = Array.ConvertAll(sampleColors, color => color.grayscale);
            var totalGreyValue = coloursAsGrey.Sum();

            var grey = Random.Range(0, totalGreyValue);
            var i = 0;
            for (; i < coloursAsGrey.Length; i++)
            {
                grey -= coloursAsGrey[i];
                if (grey <= 0)
                {
                    break;
                }
            }

            var x = minX + i % (halfSquareExtents * 2);
            var y = minY + i / (halfSquareExtents * 2);

            Vector2 targetPosition = new(x, y);
            var direction = (targetPosition - halfSize) / halfSize.x;

            return direction;
        }
    }
}