using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InsideRoomPanel : MonoBehaviour
{
    public GameObject InsideRoomUIPanel;
    public TextMeshProUGUI roomInfoText;
    public GameObject playerListPrefab;
    public GameObject playerListContent;
    public GameObject startGameButton;

    [SerializeField] private Button leaveButton;
    [SerializeField] private Button startButton;

    public event Action OnClickLeave;
    public event Action OnClickStartGame;

    private void Leave()
    {
        OnClickLeave?.Invoke();
    }

    private void StartGame()
    {
        OnClickStartGame?.Invoke();
    }

    private void Start()
    {
        leaveButton.onClick.AddListener(Leave);
        startButton.onClick.AddListener(StartGame);
    }
}
