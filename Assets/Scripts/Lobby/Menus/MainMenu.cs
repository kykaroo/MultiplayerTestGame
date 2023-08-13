using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button randomRoomButton;
    [SerializeField] private Button joinByIdButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button roomListButton;
    [SerializeField] private Button optionsButton;
    
    [SerializeField] private Menu menu;
    
    public Menu Menu => menu;
    
    public event Action OnClickCreateRoom;
    public event Action OnClickRandomRoom;
    public event Action OnClickJoinById;
    public event Action OnClickExit;
    public event Action OnClickRoomList;
    public event Action OnClickOptions;

    protected virtual void CreateRoom()
    {
        OnClickCreateRoom?.Invoke();
    }
    
    protected virtual void RandomRoom()
    {
        OnClickRandomRoom?.Invoke();
    }

    protected virtual void JoinById()
    {
        OnClickJoinById?.Invoke();
    }

    protected virtual void Exit()
    {
        OnClickExit?.Invoke();
    }

    protected virtual void RoomList()
    {
        OnClickRoomList?.Invoke();
    }
    
    protected virtual void Options()
    {
        OnClickOptions?.Invoke();
    }

    private void Start()
    {
        createRoomButton.onClick.AddListener(CreateRoom);
        randomRoomButton.onClick.AddListener(RandomRoom);
        joinByIdButton.onClick.AddListener(JoinById);
        exitButton.onClick.AddListener(Exit);
        roomListButton.onClick.AddListener(RoomList);
        optionsButton.onClick.AddListener(Options);
    }
}
