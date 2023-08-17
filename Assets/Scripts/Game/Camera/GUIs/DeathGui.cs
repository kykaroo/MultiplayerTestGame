using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Button = UnityEngine.UI.Button;

namespace Game.Camera.GUIs
{
    public class DeathGui : MonoBehaviour
    {
        [SerializeField] private Button respawnButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private TextMeshProUGUI respawnText; 
        [SerializeField] private float respawnTime;
    
        private float _respawnTimer;

        public event Action OnQuitButtonClick;
        public event Action OnRespawnButtonClick;

        protected virtual void OnRespawnClick()
        {
            OnRespawnButtonClick?.Invoke();
        }

        protected virtual void OnQuitClick()
        {
            OnQuitButtonClick?.Invoke();
        }
    
        private void Start()
        {
            _respawnTimer = respawnTime;
            quitButton.onClick.AddListener(OnQuitClick);
            respawnButton.onClick.AddListener(OnRespawnClick);
        }
        
        private void Update()
        {
            _respawnTimer -= Time.deltaTime;
            respawnTime = (float)Math.Round(_respawnTimer, 1);
        
            if (_respawnTimer <= 0)
            {
                _respawnTimer = 0;
                respawnText.text = "Respawn";
                respawnButton.interactable = true;
            }
            else
            {
                respawnButton.interactable = false;
                respawnText.text = $"Respawn: {respawnTime}";
            }
        }
    }
}
