using UnityEngine;

namespace Game.Spawn
{
    public class SpawnPoint : MonoBehaviour
    {
        [SerializeField] private GameObject graphics;

        private void Awake()
        {
            graphics.SetActive(false);
        }
    }
}
