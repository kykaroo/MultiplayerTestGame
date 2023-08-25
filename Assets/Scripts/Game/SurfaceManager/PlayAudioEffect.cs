using System.Collections.Generic;
using UnityEngine;

namespace Game.SurfaceManager
{
    [CreateAssetMenu(menuName = "Impact System/Play Audio Effect", fileName = "PlayAudioEffect")]
    public class PlayAudioEffect : ScriptableObject
    {
        public AudioSource audioSourcePrefab;
        public string audioSourcePrefabPath;
        public List<AudioClip> audioClips = new();
        [Tooltip("Значения между 0 и 1")] 
        public Vector2 volumeRange = new(0, 1);
    }
}