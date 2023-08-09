using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Photon.Realtime;
using Random = UnityEngine.Random;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance;
    
    private void Awake()
    {
        Instance = this;
    }
    
    public TextMeshProUGUI connectionStatusText;
    
    private Dictionary<string, RoomInfo> _cachedRoomList;
    private Dictionary<string, RoomListItem> _roomListGameObjects;

    [SerializeField] private MenuManager menuManager;
    private CreateRoomMenu _createRoomMenu;
    private InsideRoomMenu _insideRoomMenu;
    private JoinByIDMenu _joinByIDMenu;
    private JoinRandomRoomMenu _joinRandomRoomMenu;
    private LoginMenu _loginMenu;
    private MainMenu _mainMenu;
    private RoomListMenu _roomListMenu;
    
    [SerializeField] private MenuFactory menuFactory;

    private void Start()
    { 
        _createRoomMenu = menuFactory.CreateMenuWindow<CreateRoomMenu>();
        _createRoomMenu.OnClickCreateRoom += OnRoomCreateButtonClicked;
        _createRoomMenu.OnClickBack += OnBackButtonClicked;
        menuManager.AddMenu(_createRoomMenu.Menu);

        _insideRoomMenu = menuFactory.CreateMenuWindow<InsideRoomMenu>();
        _insideRoomMenu.OnClickLeave += OnLeaveGameButtonClicked;
        _insideRoomMenu.OnClickStartGame += OnStartGameButtonClicked;
        menuManager.AddMenu(_insideRoomMenu.Menu);
        
        _joinByIDMenu = menuFactory.CreateMenuWindow<JoinByIDMenu>();
        _joinByIDMenu.OnClickJoin += () => OnJoinRoomButtonClicked(_joinByIDMenu.RoomName);
        _joinByIDMenu.OnClickBack += OnBackButtonClicked;
        menuManager.AddMenu(_joinByIDMenu.Menu);
        
        _joinRandomRoomMenu = menuFactory.CreateMenuWindow<JoinRandomRoomMenu>();
        _joinRandomRoomMenu.OnClickBack += OnBackButtonClicked;
        menuManager.AddMenu(_joinRandomRoomMenu.Menu);

        _loginMenu = menuFactory.CreateMenuWindow<LoginMenu>();
        _loginMenu.OnClickLogin += OnLoginButtonClicked;
        menuManager.AddMenu(_loginMenu.Menu);
        
        
        _mainMenu = menuFactory.CreateMenuWindow<MainMenu>();
        _mainMenu.OnClickCreateRoom += OnMainMenuCreateRoomButtonClicked;
        _mainMenu.OnClickExit += Application.Quit;
        _mainMenu.OnClickRoomList += OnShowRoomListButtonClicked;
        _mainMenu.OnClickRandomRoom += OnJoinRandomRoomButtonClicked;
        _mainMenu.OnClickJoinById += () => menuManager.OpenMenu(_joinByIDMenu.Menu);
        menuManager.AddMenu(_mainMenu.Menu);

        _roomListMenu = menuFactory.CreateMenuWindow<RoomListMenu>();
        _roomListMenu.OnClickBackButton += OnBackButtonClicked;
        menuManager.AddMenu(_roomListMenu.Menu);
        
        _cachedRoomList = new();  
        _roomListGameObjects = new();
        
        menuManager.OpenMenu(_loginMenu.Menu);
    }

    private void Update()
    {
        connectionStatusText.text = "Connection status: " + PhotonNetwork.NetworkClientState;
    }
    
    public override void OnConnected()
    {
        print("Connected to internet");
    }

    public override void OnConnectedToMaster()
    {
        print(PhotonNetwork.LocalPlayer.NickName + " is connected to photon");
        menuManager.OpenMenu(_mainMenu.Menu);
    }

    public override void OnCreatedRoom()
    {
        print(PhotonNetwork.CurrentRoom.Name + " is created");
    }

    public override void OnJoinedRoom()
    {
        print(PhotonNetwork.LocalPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name);
        menuManager.OpenMenu(_insideRoomMenu.Menu);
        
        _insideRoomMenu.TryActivateStartButton();

        _insideRoomMenu.roomInfoText.text =
            $"Room name: {PhotonNetwork.CurrentRoom.Name} \n\r  Players: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
        
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            PlayerListItem playerListItem = _insideRoomMenu.CreatePlayerListItem();
            playerListItem.SetUp(player);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListView();

        foreach (var room in roomList)
        {
            if (!room.IsOpen || !room.IsVisible || room.RemovedFromList)
            {
                if (_cachedRoomList.ContainsKey(room.Name))
                {
                    _cachedRoomList.Remove(room.Name);
                }
            }
            else
            {
                _cachedRoomList[room.Name] = room;
            }
        }

        foreach (RoomInfo room in _cachedRoomList.Values)
        {
            RoomListItem roomListItem = _roomListMenu.CreateRoomListItem();
            roomListItem.SetUp(room);
            
            roomListItem.OnClickJoinRoom += () => OnJoinRoomButtonClicked(room.Name);

            _roomListGameObjects.Add(room.Name, roomListItem);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        _insideRoomMenu.roomInfoText.text =
            $"Room name: {PhotonNetwork.CurrentRoom.Name} \n\r  Players: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";

        PlayerListItem playerListItem = _insideRoomMenu.CreatePlayerListItem();
        playerListItem.SetUp(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        _insideRoomMenu.roomInfoText.text =
            $"Room name: {PhotonNetwork.CurrentRoom.Name} \n\r  Players: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
    }
    
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print(message);

        string roomName = $"Room {Random.Range(1000, 10000)}";

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 20
        };
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }
    
    public override void OnLeftRoom()
    {
        menuManager.OpenMenu(_mainMenu.Menu);
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
        foreach (RoomListItem roomListGameObject in _roomListGameObjects.Values)
        {
            Destroy(roomListGameObject.gameObject);
        }
        
        _roomListGameObjects.Clear();
    }

    private void OnJoinRoomButtonClicked(string roomName)
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        
        PhotonNetwork.JoinRoom(roomName);
    }

    private void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    private void OnBackButtonClicked()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        menuManager.OpenMenu(_mainMenu.Menu);
    }

    private void OnJoinByIDButtonClicked()
    {
        
    }

    private void OnJoinRandomRoomButtonClicked()
    {
        menuManager.OpenMenu(_joinRandomRoomMenu.Menu);
        PhotonNetwork.JoinRandomRoom();
    }

    private void OnStartGameButtonClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Game");
        }
    }

    private void OnLoginButtonClicked()
    {
        var playerName = _loginMenu.PlayerName;
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
        var roomName = _createRoomMenu.RoomName;

        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "Room " + Random.Range(1000, 9999);
        }

        var roomOptions = new RoomOptions
        {
            MaxPlayers = (byte)int.Parse(_createRoomMenu.MaxPlayer)
        };

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void OnCancelButtonClicked()
    {
        menuManager.OpenMenu(_mainMenu.Menu);
    }

    public void OnShowRoomListButtonClicked()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
        
        menuManager.OpenMenu(_roomListMenu.Menu);
    }

    private void OnMainMenuCreateRoomButtonClicked()
    {
        menuManager.OpenMenu(_createRoomMenu.Menu);
    }
}
