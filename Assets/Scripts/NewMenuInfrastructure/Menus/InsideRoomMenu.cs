using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InsideRoomMenu : MonoBehaviour
{
    private GameObject playerListPrefab;
    [SerializeField] public GameObject playerListContent;
    [SerializeField] public GameObject startGameButton;

    [SerializeField] private Button leaveButton;
    [SerializeField] private Button startButton;
    
    [SerializeField] public TextMeshProUGUI roomInfoText;
    
    [SerializeField] private Menu menu;

    public string RoomInfo => roomInfoText.text;
    public GameObject PlayerListPrefab => playerListPrefab;
    public GameObject PlayerListContent => playerListContent;
    public GameObject StartGameButton => startGameButton;
    public Menu Menu => menu;

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

    private void Awake()
    {
        playerListPrefab = Resources.Load<GameObject>("ItemForListPrefabs/PlayerListItemPrefab");
    }

    public GameObject CreatePlayerListItem()
    {
        GameObject playerListGameObject = Instantiate(playerListPrefab);
        playerListGameObject.transform.SetParent(playerListContent.transform);
        playerListGameObject.transform.localScale = Vector3.one;
        playerListGameObject.transform.localPosition = playerListContent.transform.position;
        return playerListGameObject;
    }

    public bool TryActivateStartButton()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            startGameButton.SetActive(true);
            return true;
        }
        else
        {
            startGameButton.SetActive(false);
            return false;
        }
    }
}
