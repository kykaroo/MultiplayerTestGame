using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Button = UnityEngine.UI.Button;

public class DeathCamera : MonoBehaviour
{
    [SerializeField] private Button respawnButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private float respawnTimer;
    [SerializeField] private float respawnTime;
    [SerializeField] private TextMeshProUGUI respawnText;

    public event Action OnRespawnButtonClick;
    public event Action OnQuitButtonClick;

    protected virtual void OnRespawnClick()
    {
        OnRespawnButtonClick?.Invoke();
    }
    
    protected virtual void OnQuitClick()
    {
        OnQuitButtonClick?.Invoke();
    }

    private void Update()
    {
        respawnTimer -=  Time.deltaTime;
        respawnTime = (float)Math.Round(respawnTimer, 1);
        
        if (respawnTimer <= 0)
        {
            respawnTimer = 0;
            respawnText.text = "Respawn";
            respawnButton.interactable = true;
        }
        else
        {
            respawnButton.interactable = false;
            respawnText.text = $"Respawn: {respawnTime}";
        }
    }

    private void Start()
    {
        respawnButton.onClick.AddListener(OnRespawnClick);
        quitButton.onClick.AddListener(OnQuitClick);
        
        
    }
}
