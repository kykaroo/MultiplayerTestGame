using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Player
{
    public class SpawnManager : MonoBehaviour
    {
        public static SpawnManager Instance;

        [SerializeField] private SpawnPoint[] spawnPoints;

        private void Awake()
        {
            Instance = this;
            spawnPoints = GetComponentsInChildren<SpawnPoint>();
        }

        public Transform GetSpawnPoint()
        {
            return spawnPoints[Random.Range(0, spawnPoints.Length)].transform;
        }
    }
}
