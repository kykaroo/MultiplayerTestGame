using UnityEngine;

namespace Game.SurfaceManager
{
    [CreateAssetMenu(menuName = "Impact System/Spawn Object Effect", fileName = "SpawnObjectEffect")]
    public class SpawnObjectEffect : ScriptableObject
    {
        public GameObject prefab;
        public string prefabPath;
        public float probability;
        public bool randomizeRotation;
        [Tooltip("Нулевые значения запретят вращение по оси. Рекомендуемый предел значений: 360")]
        public Vector3 randomizedRotationMultiplier = Vector3.zero;
    }
}