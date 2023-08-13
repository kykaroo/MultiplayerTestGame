using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    
    [SerializeField] private List<Menu> menuList = new();
    
    [SerializeField] private MenuFactory menuFactory;

    internal InsideRoomMenu InsideRoomMenu;
    internal MainMenu MainMenu;
    internal RoomListMenu RoomListMenu;
    
    private CreateRoomMenu _createRoomMenu;
    private JoinByIDMenu _joinByIDMenu;
    private JoinRandomRoomMenu _joinRandomRoomMenu;
    private LoginMenu _loginMenu;
    private OptionsMenu _optionsMenu;
    private void Awake()
    {
        Instance = this;
    }

    public void AddMenu(Menu menu)
    {
        menuList.Add(menu);
    }

    public void OpenMenu(string menuName)
    {
        foreach (Menu t in menuList)
        {
            if (t.menuName == menuName)
            {
                t.Open();
            }
            else if (t.open)
            {
                CloseMenu(t);
            }
        }
    }

    private void CloseMenu(Menu menu)
    {
        menu.Close();
    }

    public void OpenMenu(Menu menu)
    {
        foreach (var t in menuList.Where(t => t.open))
        {
            CloseMenu(t);
        }
        menu.Open();
    }

    private void Start()
    {
        CreateCreateRoomMenu();
        CreateInsideRoomMenu();
        CreateJoinByIDMenu();
        CreateJoinRandomRoomMenu();
        CreateLoginMenu();
        CreateMainMenu();
        CreateRoomListMenu();
        CreateOptionsMenu();
        
        menuList.Add(_createRoomMenu.Menu);
        menuList.Add(InsideRoomMenu.Menu);
        menuList.Add(_joinByIDMenu.Menu);
        menuList.Add(_joinRandomRoomMenu.Menu);
        menuList.Add(_loginMenu.Menu);
        menuList.Add(MainMenu.Menu);
        menuList.Add(RoomListMenu.Menu);
        menuList.Add(_optionsMenu.Menu);
        
        OpenMenu(_loginMenu.Menu);
    }

    #region CreateMenu
    
    private void CreateRoomListMenu()
    {
        RoomListMenu = menuFactory.CreateMenuWindow<RoomListMenu>();
        RoomListMenu.OnClickBackButton += OnBackButtonClicked;
    }

    private void CreateMainMenu()
    {
        MainMenu = menuFactory.CreateMenuWindow<MainMenu>();
        MainMenu.OnClickCreateRoom += OnMainMenuCreateRoomButtonClicked;
        MainMenu.OnClickExit += Application.Quit;
        MainMenu.OnClickRoomList += OnShowRoomListButtonClicked;
        MainMenu.OnClickRandomRoom += OnJoinRandomRoomButtonClicked;
        MainMenu.OnClickJoinById += () => OpenMenu(_joinByIDMenu.Menu);
        MainMenu.OnClickOptions += OnOptionsButtonClicked;
    }

    private void CreateLoginMenu()
    {
        _loginMenu = menuFactory.CreateMenuWindow<LoginMenu>();
        _loginMenu.OnClickLogin += OnLoginButtonClicked;
    }

    private void CreateJoinRandomRoomMenu()
    {
        _joinRandomRoomMenu = menuFactory.CreateMenuWindow<JoinRandomRoomMenu>();
        _joinRandomRoomMenu.OnClickBack += OnBackButtonClicked;
    }

    private void CreateJoinByIDMenu()
    {
        _joinByIDMenu = menuFactory.CreateMenuWindow<JoinByIDMenu>();
        _joinByIDMenu.OnClickJoin += () => OnJoinRoomButtonClicked(_joinByIDMenu.RoomName);
        _joinByIDMenu.OnClickBack += OnBackButtonClicked;
    }

    private void CreateInsideRoomMenu()
    {
        InsideRoomMenu = menuFactory.CreateMenuWindow<InsideRoomMenu>();
        InsideRoomMenu.OnClickLeave += OnLeaveGameButtonClicked;
        InsideRoomMenu.OnClickStartGame += OnStartGameButtonClicked;
    }

    private void CreateCreateRoomMenu()
    {
        _createRoomMenu = menuFactory.CreateMenuWindow<CreateRoomMenu>();
        _createRoomMenu.OnClickCreateRoom += OnRoomCreateButtonClicked;
        _createRoomMenu.OnClickBack += OnBackButtonClicked;
    }

    private void CreateOptionsMenu()
    {
        _optionsMenu = menuFactory.CreateMenuWindow<OptionsMenu>();
        _optionsMenu.OnClickBack += OnBackButtonClicked;
    }

    #endregion

    #region OnButtonClicks

    internal static void OnJoinRoomButtonClicked(string roomName)
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        
        PhotonNetwork.JoinRoom(roomName);
    }

    private static void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    private void OnBackButtonClicked()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        OpenMenu(MainMenu.Menu);
    }
    
    private void OnJoinRandomRoomButtonClicked()
    {
        OpenMenu(_joinRandomRoomMenu.Menu);
        PhotonNetwork.JoinRandomRoom();
    }

    private static void OnStartGameButtonClicked()
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
        OpenMenu(MainMenu.Menu);
    }

    public void OnShowRoomListButtonClicked()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
        
        OpenMenu(RoomListMenu.Menu);
    }

    private void OnMainMenuCreateRoomButtonClicked()
    {
        OpenMenu(_createRoomMenu.Menu);
    }

    private void OnOptionsButtonClicked()
    {
        OpenMenu(_optionsMenu.Menu);
    }

    #endregion
}
