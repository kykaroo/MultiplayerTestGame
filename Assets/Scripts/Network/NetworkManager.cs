using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Photon.Realtime;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header(("Connection Status"))] public TextMeshProUGUI ConnectionStatusText;
    
    [Header("Login UI Panel")] public TMP_InputField playerNameInput;
    public GameObject LoginUIPanel;

    [Header("Main Menu UI Panel")] public GameObject mainMenuUIPanel;

    [Header("Create Room UI Panel")] public GameObject CreateRoomUIPanel;
    public TMP_InputField roomNameInputField;
    public TMP_InputField maxPlayersInputField;

    [Header("Inside Room UI Panel")] public GameObject InsideRoomUIPanel;
    public TextMeshProUGUI roomInfoText;
    public GameObject playerListPrefab;
    public GameObject playerListContent;

    [Header("Room List UI Panel")] public GameObject RoomListUIPanel;
    public GameObject roomListEntryPrefab;
    public GameObject roomListParentGameObject;

    [Header("Join Random Room UI Panel")] public GameObject JoinRandomRoomUIPanel;

    [Header("Join By ID Panel")] public GameObject JoinByIdUIPanel;
    public TMP_InputField roomIDInputField;

    private Dictionary<string, RoomInfo> _cachedRoomList;
    private Dictionary<string, GameObject> roomListGameObjects;
    private Dictionary<int, GameObject> playerListGameObjects;

    private void Start()
    {
        ActivePanel(LoginUIPanel.name);
        
        _cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListGameObjects = new Dictionary<string, GameObject>();
    }

    private void Update()
    {
        ConnectionStatusText.text = "Connection status: " + PhotonNetwork.NetworkClientState;
    }

    public void OnLoginButtonClicked()
    {
        string playerName = playerNameInput.text;
        if (!string.IsNullOrEmpty(playerName))
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            print("Player name is invalid!");
        }
    }

    public void OnRoomCreateButtonClicked()
    {
        string roomName = roomNameInputField.text;

        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "Room " + Random.Range(1000, 9999);
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)int.Parse(maxPlayersInputField.text);

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void OnCancelButtonClicked()
    {
        ActivePanel(mainMenuUIPanel.name);
    }

    public void OnShowRoomListButtonClicked()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
        
        ActivePanel(RoomListUIPanel.name);
    }

    public override void OnConnected()
    {
        print("Connected to internet");
    }

    public override void OnConnectedToMaster()
    {
        print(PhotonNetwork.LocalPlayer.NickName + " is connected to photon");
        ActivePanel(mainMenuUIPanel.name);
    }

    public override void OnCreatedRoom()
    {
        print(PhotonNetwork.CurrentRoom.Name + " is created");
    }

    public override void OnJoinedRoom()
    {
        print(PhotonNetwork.LocalPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name);
        ActivePanel(InsideRoomUIPanel.name);

        roomInfoText.text =
            $"Room name: {PhotonNetwork.CurrentRoom.Name} \n\r  Players: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";

        if (playerListGameObjects == null)
        {
            playerListGameObjects = new Dictionary<int, GameObject>();
        }

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerListGameObject = Instantiate(playerListPrefab);
            playerListGameObject.transform.SetParent(playerListContent.transform);
            playerListGameObject.transform.localScale = Vector3.one;
            playerListGameObject.transform.position = playerListContent.transform.position;

            playerListGameObject.transform.Find("PlayerNameText").GetComponent<TextMeshProUGUI>().text =
                player.NickName;

            if (player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                playerListGameObject.transform.Find("PlayerIndicator").gameObject.SetActive(true);
            }
            else
            {
                playerListGameObject.transform.Find("PlayerIndicator").gameObject.SetActive(false);
            }
            
            playerListGameObjects.Add(player.ActorNumber, playerListGameObject);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListView();

        foreach (RoomInfo room in roomList)
        {
            print(room.Name);
            if (!room.IsOpen || !room.IsVisible || room.RemovedFromList)
            {
                if (_cachedRoomList.ContainsKey(room.Name))
                {
                    _cachedRoomList.Remove(room.Name);
                }
            }
            else
            {
                if (_cachedRoomList.ContainsKey(room.Name))
                {
                    _cachedRoomList[room.Name] = room;
                }
                else
                {
                    _cachedRoomList.Add(room.Name, room);
                }
            }
        }

        foreach (RoomInfo room in _cachedRoomList.Values)
        {
            GameObject roomListEntryGameObject = Instantiate(roomListEntryPrefab);
            roomListEntryGameObject.transform.SetParent(roomListParentGameObject.transform);
            roomListEntryGameObject.transform.localScale = Vector3.one;
            roomListEntryGameObject.transform.localPosition = roomListParentGameObject.transform.position;

            roomListEntryGameObject.transform.Find("RoomNameText").GetComponent<TextMeshProUGUI>().text = room.Name;
            roomListEntryGameObject.transform.Find("RoomPlayersText").GetComponent<TextMeshProUGUI>().text = room.PlayerCount + " / " + room.MaxPlayers;
            roomListEntryGameObject.transform.Find("JoinRoomButton").GetComponent<Button>().onClick.AddListener((() => OnJoinRoomButtonClicked(room.Name)));
            
            roomListGameObjects.Add(room.Name, roomListEntryGameObject);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        roomInfoText.text =
            $"Room name: {PhotonNetwork.CurrentRoom.Name} \n\r  Players: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";

        GameObject playerListGameObject = Instantiate(playerListPrefab);
        playerListGameObject.transform.SetParent(playerListContent.transform);
        playerListGameObject.transform.localScale = Vector3.one;
        playerListGameObject.transform.localPosition = playerListContent.transform.position;

        playerListGameObject.transform.Find("PlayerNameText").GetComponent<TextMeshProUGUI>().text = newPlayer.NickName;

        if (newPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            playerListGameObject.transform.Find("PlayerIndicator").gameObject.SetActive(true);
        }
        else
        {
            playerListGameObject.transform.Find("PlayerIndicator").gameObject.SetActive(false);
        }
        
        playerListGameObjects.Add(newPlayer.ActorNumber, playerListGameObject);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        roomInfoText.text =
            $"Room name: {PhotonNetwork.CurrentRoom.Name} \n\r  Players: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
        
        Destroy(playerListGameObjects[otherPlayer.ActorNumber].gameObject);
        playerListGameObjects.Remove(otherPlayer.ActorNumber);
    }

    public override void OnLeftRoom()
    {
        ActivePanel(mainMenuUIPanel.name);

        foreach (GameObject playerListGameObject in playerListGameObjects.Values)
        {
            Destroy(playerListGameObject);
        }
        
        playerListGameObjects.Clear();
        playerListGameObjects = null;
    }

    public void OnJoinRoomButtonClicked(string roomName)
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        PhotonNetwork.JoinRoom(roomName);
    }

    void ClearRoomListView()
    {
        foreach (GameObject roomListGameObject in roomListGameObjects.Values)
        {
            Destroy(roomListGameObject);
        }
        
        roomListGameObjects.Clear();
    }

    public void ActivePanel(string panelToBeActivated)
    {
        LoginUIPanel.SetActive(panelToBeActivated.Equals(LoginUIPanel.name));
        mainMenuUIPanel.SetActive(panelToBeActivated.Equals(mainMenuUIPanel.name));
        CreateRoomUIPanel.SetActive(panelToBeActivated.Equals(CreateRoomUIPanel.name));
        InsideRoomUIPanel.SetActive(panelToBeActivated.Equals(InsideRoomUIPanel.name));
        RoomListUIPanel.SetActive(panelToBeActivated.Equals(RoomListUIPanel.name));
        JoinByIdUIPanel.SetActive(panelToBeActivated.Equals(JoinByIdUIPanel.name));
        JoinRandomRoomUIPanel.SetActive(panelToBeActivated.Equals(JoinRandomRoomUIPanel.name));
    }
}
