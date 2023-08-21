using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.ItemSystem.NewSystem;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Shoot Config", menuName = "Guns/Shoot Config")]
public class ShootConfig : ScriptableObject
{
    public LayerMask HitMask;
    public float FireRate = 0.25f;
    public float RecoilRecoverySpeed = 1f;
    public float MaxSpreadTime = 1f;
    public BulletSpreadType SpreaType = BulletSpreadType.Simple;
    [Header("Simple Spread")]
    public Vector3 Spread = new(0.1f, 0.1f, 0.1f);

    [Header("Texture-Based spread")] [Range(0.001f, 5f)]
    public float SpreadMultiplier = 0.1f;
    public Texture2D SpreatTexture;

    public Vector3 GetSpread(float ShootTime = 0)
    {
        Vector3 spread = Vector3.zero;

        if (SpreaType == BulletSpreadType.Simple )
        {
            spread = Vector3.Lerp(Vector3.zero,
                new(Random.Range(-Spread.x, Spread.x), Random.Range(-Spread.y, Spread.y),
                    Random.Range(-Spread.z, Spread.z)), Mathf.Clamp01(ShootTime / MaxSpreadTime));
        }
        else
        {
            if (SpreaType == BulletSpreadType.TextureBased)
            {
                spread = GetTextureDirection(ShootTime);
                spread *= SpreadMultiplier;
            }
        }
        
        return spread;
    }

    private Vector3 GetTextureDirection(float shootTime)
    {
        Vector2 halfSize = new(SpreatTexture.width / 2f, SpreatTexture.height / 2f);
        int halfSquareExtents = Mathf.CeilToInt(Mathf.Lerp(0.01f, halfSize.x, Mathf.Clamp01(shootTime / MaxSpreadTime)));
        
        int minX = Mathf.FloorToInt(halfSize.x) - halfSquareExtents;
        int minY = Mathf.FloorToInt(halfSize.y) - halfSquareExtents;

        Color[] sampleColors = SpreatTexture.GetPixels(minX, minY, halfSquareExtents * 2, halfSquareExtents * 2);

        float[] coloursAsGrey = Array.ConvertAll(sampleColors, color => color.grayscale);
        float totalGreyValue = coloursAsGrey.Sum();

        float grey = Random.Range(0, totalGreyValue);
        int i = 0;
        for (; i < coloursAsGrey.Length; i++)
        {
            grey -= coloursAsGrey[i];
            if (grey <= 0)
            {
                break;
            }
        }

        int x = minX + i % (halfSquareExtents * 2);
        int y = minY + i / (halfSquareExtents * 2);

        Vector2 targetPosition = new(x, y);
        Vector2 direction = (targetPosition - halfSize) / halfSize.x;

        return direction;
    }
}
