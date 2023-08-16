using UnityEngine;

namespace Game.Player
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
