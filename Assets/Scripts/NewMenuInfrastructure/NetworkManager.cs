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
    public static NetworkManager Instance;
    
    private void Awake()
    {
        Instance = this;
    }
    
    public TextMeshProUGUI ConnectionStatusText;
    
    private Dictionary<string, RoomInfo> _cachedRoomList;
    private Dictionary<string, GameObject> roomListGameObjects;
    private Dictionary<int, GameObject> playerListGameObjects;

    [SerializeField] private MenuManager _menuManager;
    private CreateRoomMenu _createRoomMenu;
    private InsideRoomMenu _insideRoomMenu;
    private JoinByIDMenu _joinByIDMenu;
    private JoinRandomRoomMenu _joinRandomRoomMenu;
    private LoginMenu _loginMenu;
    private MainMenu _mainMenu;
    private RoomListMenu _roomListMenu;

    [SerializeField] private Transform rootForUI;
    
    [SerializeField] private MenuFactory _menuFactory;

    private void Start()
    { 
        _createRoomMenu = _menuFactory.CreateMenuWindow<CreateRoomMenu>();
        _createRoomMenu.OnClickCreateRoom += OnRoomCreateButtonClicked;
        _createRoomMenu.OnClickBack += OnBackButtonClicked;
        _menuManager.AddMenu(_createRoomMenu.Menu);

        _insideRoomMenu = _menuFactory.CreateMenuWindow<InsideRoomMenu>();
        _insideRoomMenu.OnClickLeave += OnLeaveGameButtonClicked;
        _insideRoomMenu.OnClickStartGame += OnStartGameButtonClicked;
        _menuManager.AddMenu(_insideRoomMenu.Menu);
        
        _joinByIDMenu = _menuFactory.CreateMenuWindow<JoinByIDMenu>();
        _joinByIDMenu.OnClickJoin += () => OnJoinRoomButtonClicked(_joinByIDMenu.roomName);
        _joinByIDMenu.OnClickBack += OnBackButtonClicked;
        _menuManager.AddMenu(_joinByIDMenu.Menu);
        
        _joinRandomRoomMenu = _menuFactory.CreateMenuWindow<JoinRandomRoomMenu>();
        _joinRandomRoomMenu.OnClickBack += OnBackButtonClicked;
        _menuManager.AddMenu(_joinRandomRoomMenu.Menu);

        _loginMenu = _menuFactory.CreateMenuWindow<LoginMenu>();
        _loginMenu.OnClickLogin += OnLoginButtonClicked;
        _menuManager.AddMenu(_loginMenu.Menu);
        
        
        _mainMenu = _menuFactory.CreateMenuWindow<MainMenu>();
        _mainMenu.OnClickCreateRoom += OnMainMenuCreateRoomButtonClicked;
        _mainMenu.OnClickExit += Application.Quit;
        _mainMenu.OnClickRoomList += OnShowRoomListButtonClicked;
        _mainMenu.OnClickRandomRoom += OnJoinRandomRoomButtonClicked;
        _mainMenu.OnClickJoinById += () => _menuManager.OpenMenu(_joinByIDMenu.Menu);
        _menuManager.AddMenu(_mainMenu.Menu);

        _roomListMenu = _menuFactory.CreateMenuWindow<RoomListMenu>();
        _roomListMenu.OnClickBackButton += OnBackButtonClicked;
        _menuManager.AddMenu(_roomListMenu.Menu);
        
        _cachedRoomList = new Dictionary<string, RoomInfo>();  
        roomListGameObjects = new Dictionary<string, GameObject>();  
	  
        _menuManager.OpenMenu(_loginMenu.Menu);
    }

    private void Update()
    {
        ConnectionStatusText.text = "Connection status: " + PhotonNetwork.NetworkClientState;
    }
    
    public override void OnConnected()
    {
        print("Connected to internet");
    }

    public override void OnConnectedToMaster()
    {
        print(PhotonNetwork.LocalPlayer.NickName + " is connected to photon");
        _menuManager.OpenMenu(_mainMenu.Menu);
    }

    public override void OnCreatedRoom()
    {
        print(PhotonNetwork.CurrentRoom.Name + " is created");
    }

    public override void OnJoinedRoom()
    {
        print(PhotonNetwork.LocalPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name);
        _menuManager.OpenMenu(_insideRoomMenu.Menu);
        
        _insideRoomMenu.TryActivateStartButton();

        _insideRoomMenu.roomInfoText.text =
            $"Room name: {PhotonNetwork.CurrentRoom.Name} \n\r  Players: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";

        if (playerListGameObjects == null)
        {
            playerListGameObjects = new Dictionary<int, GameObject>();
        }
        

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerListGameObject = _insideRoomMenu.CreatePlayerListItem();

            PlayerListItem playerListItem = playerListGameObject.GetComponent<PlayerListItem>();
            playerListItem.SetUp(player);

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
            GameObject roomListEntryGameObject = _roomListMenu.CreateRoomListItem();
            
            RoomListItem roomListItem = roomListEntryGameObject.GetComponent<RoomListItem>();
            roomListItem.SetUp(room);
            
            roomListItem.OnClickJoinRoom += () => OnJoinRoomButtonClicked(room.Name);

            roomListGameObjects.Add(room.Name, roomListEntryGameObject);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        _insideRoomMenu.roomInfoText.text =
            $"Room name: {PhotonNetwork.CurrentRoom.Name} \n\r  Players: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";

        GameObject playerListGameObject = _insideRoomMenu.CreatePlayerListItem();

        PlayerListItem playerListItem = playerListGameObject.GetComponent<PlayerListItem>();
        playerListItem.SetUp(newPlayer);

        playerListGameObjects.Add(newPlayer.ActorNumber, playerListGameObject);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        _insideRoomMenu.roomInfoText.text =
            $"Room name: {PhotonNetwork.CurrentRoom.Name} \n\r  Players: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
        
        Destroy(playerListGameObjects[otherPlayer.ActorNumber].gameObject);
        playerListGameObjects.Remove(otherPlayer.ActorNumber);
    }

    public override void OnLeftRoom()
    {
        _menuManager.OpenMenu(_mainMenu.Menu);

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

    public override void OnMasterClientSwitched(Player newMaserClient)
    {
        _insideRoomMenu.startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnJoinRoomFailed(short s, string room)
    {
        print($"Room \"{room}\" does not exist");
    }
    
    void ClearRoomListView()
    {
        foreach (GameObject roomListGameObject in roomListGameObjects.Values)
        {
            Destroy(roomListGameObject);
        }
        
        roomListGameObjects.Clear();
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

        _menuManager.OpenMenu(_mainMenu.Menu);
    }

    public void OnJoinByIDButtonClicked()
    {
        
    }

    public void OnJoinRandomRoomButtonClicked()
    {
        _menuManager.OpenMenu(_joinRandomRoomMenu.Menu);
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnStartGameButtonClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Game");
        }
    }

    public void OnLoginButtonClicked()
    {
        string playerName = _loginMenu.playerName;
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
        string roomName = _createRoomMenu.roomName;

        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "Room " + Random.Range(1000, 9999);
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)int.Parse(_createRoomMenu.maxPlayer);

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void OnCancelButtonClicked()
    {
        _menuManager.OpenMenu(_mainMenu.Menu);
    }

    public void OnShowRoomListButtonClicked()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
        
        _menuManager.OpenMenu(_roomListMenu.Menu);
    }
    
    public void OnMainMenuCreateRoomButtonClicked()
    {
        _menuManager.OpenMenu(_createRoomMenu.Menu);
    }
}
