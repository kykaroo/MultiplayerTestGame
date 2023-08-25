using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace Game.SurfaceManager
{
    public class SurfaceManager : MonoBehaviour
    {
        private static SurfaceManager _instance;

        public static SurfaceManager Instance
        {
            get
            {
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        [SerializeField] private List<SurfaceType> surfaces = new();
        [SerializeField] private Surface defaultSurface;

        public void HandleImpact(GameObject hitObject, Vector3 hitPoint, Vector3 hitNormal, ImpactType impact, int triangleIndex)
        {
            if (hitObject.TryGetComponent<Terrain>(out var terrain))
            {
                List<TextureAlpha> activeTextures = GetActiveTexturesFromTerrain(terrain, hitPoint);

                foreach (var activeTexture in activeTextures)
                {
                    SurfaceType surfaceType = surfaces.Find(surface => surface.Albedo == activeTexture.Texture);
                    if (surfaceType != null)
                    {
                        foreach (var typeEffect in surfaceType.Surface.impactTypeEffects)
                        {
                            if (typeEffect.ImpactType == impact)
                            {
                                PlayEffects(hitPoint, hitNormal, typeEffect.SurfaceEffect, activeTexture.Alpha);
                            }
                        }
                    }
                    else
                    {
                        foreach (var typeEffect in defaultSurface.impactTypeEffects)
                        {
                            if (typeEffect.ImpactType == impact)
                            {
                                PlayEffects(hitPoint, hitNormal, typeEffect.SurfaceEffect, 1);
                            }
                        }
                    }
                }
            }
            else if (hitObject.TryGetComponent<Renderer>(out Renderer renderer))
            {
                Texture activeTexture = GetActiveTextureFromRenderer(renderer, triangleIndex);
                SurfaceType surfaceType = surfaces.Find(surface => surface.Albedo == activeTexture);

                if (surfaceType != null)
                {
                    foreach (var typeEffect in surfaceType.Surface.impactTypeEffects)
                    {
                        if (typeEffect.ImpactType == impact)
                        {
                            PlayEffects(hitPoint, hitNormal, typeEffect.SurfaceEffect, 1);
                        }
                    }
                }
                else
                {
                    foreach (var typeEffect in defaultSurface.impactTypeEffects)
                    {
                        if (typeEffect.ImpactType == impact)
                        {
                            PlayEffects(hitPoint, hitNormal, typeEffect.SurfaceEffect, 1);
                        }
                    }
                }
            }
        }

        private List<TextureAlpha> GetActiveTexturesFromTerrain(Terrain terrain, Vector3 hitPoint)
        {
            Vector3 terrainPosition = hitPoint - terrain.transform.position;
            Vector3 splatMapPosition = new Vector3(terrainPosition.x / terrain.terrainData.size.x, 0,
                terrainPosition.z / terrain.terrainData.size.z);

            int x = Mathf.FloorToInt(splatMapPosition.x * terrain.terrainData.alphamapWidth);
            int z = Mathf.FloorToInt(splatMapPosition.z * terrain.terrainData.alphamapHeight);
            float[,,] alphaMap = terrain.terrainData.GetAlphamaps(x, z, 1, 1);
            List<TextureAlpha> activeTextures = new List<TextureAlpha>();

            for (int i = 0; i < alphaMap.Length; i++)
            {
                if (alphaMap[0, 0, i] > 0)
                {
                    activeTextures.Add(new TextureAlpha()
                    {
                        Texture = terrain.terrainData.terrainLayers[i].diffuseTexture,
                        Alpha = alphaMap[0, 0, i]
                    });
                }
            }

            return activeTextures;
        }

        private Texture GetActiveTextureFromRenderer(Renderer renderer, int triangleIndex)
        {
            if (renderer.TryGetComponent<MeshFilter>(out MeshFilter meshFilter))
            {
                Mesh mesh = meshFilter.mesh;

                return GetTextureFromMesh(mesh, triangleIndex, renderer.sharedMaterials);
            }
            else if (renderer is SkinnedMeshRenderer)
            {
                SkinnedMeshRenderer smr = (SkinnedMeshRenderer)renderer;
                Mesh mesh = smr.sharedMesh;
                
                return GetTextureFromMesh(mesh, triangleIndex, renderer.sharedMaterials);
            }

            return null;
        }

        private void PlayEffects(Vector3 hitPoint, Vector3 hitNormal, SurfaceEffect surfaceEffect, float soundOffset)
        {
            foreach (var spawnObjectEffect in surfaceEffect.SpawnObjectEffects)
            {
                if (spawnObjectEffect.probability > Random.value)
                {
                    GameObject instance = PhotonNetwork.Instantiate(spawnObjectEffect.prefabPath, hitPoint + hitNormal * 0.001f,
                        Quaternion.identity);
                    instance.transform.forward = hitNormal;

                    if (spawnObjectEffect.randomizeRotation)
                    {
                        Vector3 offset = new Vector3(
                            Random.Range(0, 180 * spawnObjectEffect.randomizedRotationMultiplier.x),
                            Random.Range(0, 180 * spawnObjectEffect.randomizedRotationMultiplier.y),
                            Random.Range(0, 180 * spawnObjectEffect.randomizedRotationMultiplier.z));
                        
                        instance.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + offset);
                    }
                }
            }

            foreach (var playAudioEffect in surfaceEffect.PlayAudioEffects)
            {
                AudioSource instance = PhotonNetwork.Instantiate(playAudioEffect.audioSourcePrefabPath, hitPoint,
                    Quaternion.identity).GetComponent<AudioSource>();

                AudioClip clip = playAudioEffect.audioClips[Random.Range(0, playAudioEffect.audioClips.Count)];
                instance.PlayOneShot(clip, soundOffset * Random.Range(playAudioEffect.volumeRange.x, playAudioEffect.volumeRange.y));
                StartCoroutine(DisableAudioSource(instance, clip.length));
            }
        }

        private IEnumerator DisableAudioSource(AudioSource audioSource, float time)
        {
            yield return new WaitForSeconds(time);
            
            PhotonNetwork.Destroy(audioSource.gameObject);
        }

        private Texture GetTextureFromMesh(Mesh mesh, int triangleIndex, Material[] materials)
        {
            if (mesh.subMeshCount > 1)
            {
                int[] hitTriangleIndices = new int[]
                {
                    mesh.triangles[triangleIndex * 3],
                    mesh.triangles[triangleIndex * 3 + 1],
                    mesh.triangles[triangleIndex * 3 + 2]
                };

                for (int i = 0; i < mesh.subMeshCount; i++)
                {
                    int[] submeshTriangles = mesh.GetTriangles(i);

                    for (int j = 0; j < submeshTriangles.Length; j++)
                    {
                        if (submeshTriangles[j] == hitTriangleIndices[0] && hitTriangleIndices[j + 1] == hitTriangleIndices[1] && hitTriangleIndices[j + 2] == hitTriangleIndices[2])
                        {
                            return materials[i].mainTexture;
                        }
                    }
                }
            }

            return materials[0].mainTexture;
        }
        
        private class TextureAlpha
        {
            public float Alpha;
            public Texture Texture;
        }
    }
}