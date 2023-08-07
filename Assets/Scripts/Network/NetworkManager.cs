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

    [Header("Room List UI Panel")] public GameObject RoomListUIPanel;
    public GameObject roomListEntryPrefab;
    public GameObject roomListParentGameObject;

    [Header("Join Random Room UI Panel")] public GameObject JoinRandomRoomUIPanel;

    [Header("Join By ID Panel")] public GameObject JoinByIdUIPanel;
    public TMP_InputField roomIDInputField;

    private Dictionary<string, RoomInfo> _cachedRoomList;
    private Dictionary<string, GameObject> roomListGameObjects;
    private Dictionary<int, GameObject> playerListGameObjects;
    
    private InsideRoomPanel _insideRoomPanel;

    public Transform rootForUI;

    private void Start()
    {
        var prefab = Resources.Load<InsideRoomPanel>("Prefabs/InsideRoomPanel");
        _insideRoomPanel = Instantiate(prefab, rootForUI);
        _insideRoomPanel.OnClickLeave += OnLeaveGameButtonClicked;
        // _insideRoomPanel.OnClickStartGame
        
        _cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListGameObjects = new Dictionary<string, GameObject>();
        
        ActivePanel(LoginUIPanel.name);
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
        ActivePanel(_insideRoomPanel.InsideRoomUIPanel.name);

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            _insideRoomPanel.startGameButton.SetActive(true);
        }
        else
        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            _insideRoomPanel.startGameButton.SetActive(false);
        }

        _insideRoomPanel.roomInfoText.text =
            $"Room name: {PhotonNetwork.CurrentRoom.Name} \n\r  Players: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";

        if (playerListGameObjects == null)
        {
            playerListGameObjects = new Dictionary<int, GameObject>();
        }
        

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerListGameObject = Instantiate(_insideRoomPanel.playerListPrefab);
            playerListGameObject.transform.SetParent(_insideRoomPanel.playerListContent.transform);
            playerListGameObject.transform.localScale = Vector3.one;
            playerListGameObject.transform.position = _insideRoomPanel.playerListContent.transform.position;

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
        _insideRoomPanel.roomInfoText.text =
            $"Room name: {PhotonNetwork.CurrentRoom.Name} \n\r  Players: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";

        GameObject playerListGameObject = Instantiate(_insideRoomPanel.playerListPrefab);
        playerListGameObject.transform.SetParent(_insideRoomPanel.playerListContent.transform);
        playerListGameObject.transform.localScale = Vector3.one;
        playerListGameObject.transform.localPosition = _insideRoomPanel.playerListContent.transform.position;

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
        _insideRoomPanel.roomInfoText.text =
            $"Room name: {PhotonNetwork.CurrentRoom.Name} \n\r  Players: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";

        if (otherPlayer.IsMasterClient)
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                _insideRoomPanel.startGameButton.SetActive(true);
            }
        }

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

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print(message);

        string roomName = $"Room {Random.Range(1000, 10000)}";

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void OnJoinRoomButtonClicked(string roomName)
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        PhotonNetwork.JoinRoom(roomName);
    }

    public void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnBackButtonClicked()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        ActivePanel(mainMenuUIPanel.name);
    }

    public void OnJoinByIDButtonClicked()
    {
        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();

        if (_cachedRoomList.ContainsKey(_insideRoomPanel.roomInfoText.text))
            OnJoinRoomButtonClicked(_insideRoomPanel.roomInfoText.text);
        else
            print("Invalid room ID");
    }

    public void OnJoinRandomRoomButtonClicked()
    {
        ActivePanel(JoinRandomRoomUIPanel.name);
        PhotonNetwork.JoinRandomRoom();
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
        _insideRoomPanel.InsideRoomUIPanel.SetActive(panelToBeActivated.Equals(_insideRoomPanel.InsideRoomUIPanel.name));
        RoomListUIPanel.SetActive(panelToBeActivated.Equals(RoomListUIPanel.name));
        JoinByIdUIPanel.SetActive(panelToBeActivated.Equals(JoinByIdUIPanel.name));
        JoinRandomRoomUIPanel.SetActive(panelToBeActivated.Equals(JoinRandomRoomUIPanel.name));
    }
}
