using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class InsideRoomMenuState : MenuStateBase
{
    private InsideRoomMenu _insideRoomMenu;
    private NetworkManager _networkManager;

    public InsideRoomMenuState(MenuFactory menuFactory, NetworkManager networkManager) : base(menuFactory)
    {
        _networkManager = networkManager;
    }

    protected override void OnEnter()
    {
        _insideRoomMenu = MenuFactory.CreateMenuWindow<InsideRoomMenu>();
        CreatePlayers();
        
        _networkManager.OnPlayerEnteredRoomEvent += OnPlayerEnteredRoom;
        _networkManager.OnPlayerLeftRoomEvent += OnPlayerLeftRoom;
        _networkManager.OnMasterClientSwitchedEvent += OnMasterClientSwitched;
        _networkManager.OnLeftRoomEvent += OnLeftRoom;
        _insideRoomMenu.OnClickLeave += OnLeaveGameButtonClicked;
        _insideRoomMenu.OnClickStartGame += OnStartGameButtonClicked;
    }

    public void OnLeftRoom()
    {
        StateMachine.SetState<MainMenuState>();
    }

    protected override void OnExit()
    {
        _networkManager.OnPlayerEnteredRoomEvent -= OnPlayerEnteredRoom;
        _networkManager.OnPlayerLeftRoomEvent -= OnPlayerLeftRoom;
        _networkManager.OnMasterClientSwitchedEvent -= OnMasterClientSwitched;
        _networkManager.OnLeftRoomEvent -= OnLeftRoom;
        _insideRoomMenu.OnClickLeave -= OnLeaveGameButtonClicked;
        _insideRoomMenu.OnClickStartGame -= OnStartGameButtonClicked;

        Object.Destroy(_insideRoomMenu.gameObject);
    }

    private void CreatePlayers()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name);

        _insideRoomMenu.TryActivateStartButton();

        _insideRoomMenu.roomInfoText.text =
            $"Room name: {PhotonNetwork.CurrentRoom.Name} \n\r  Players: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            var playerListItem = _insideRoomMenu.CreatePlayerListItem();
            playerListItem.SetUp(player);
        }
    }

    private void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    private void OnStartGameButtonClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Game");
        }
    }


    private void OnPlayerEnteredRoom(Player newPlayer)
    {
        _insideRoomMenu.roomInfoText.text =
            $"Room name: {PhotonNetwork.CurrentRoom.Name} \n\r  Players: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";

        var playerListItem = _insideRoomMenu.CreatePlayerListItem();
        playerListItem.SetUp(newPlayer);
    }

    private void OnPlayerLeftRoom(Player otherPlayer)
    {
        _insideRoomMenu.roomInfoText.text =
            $"Room name: {PhotonNetwork.CurrentRoom.Name} \n\r  Players: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
    }

    private void OnMasterClientSwitched(Player _)
    {
        _insideRoomMenu.startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }
}