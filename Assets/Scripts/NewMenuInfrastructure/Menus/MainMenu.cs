using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button CreateRoomButton;
    [SerializeField] private Button RandomRoomButton;
    [SerializeField] private Button JoinByIdButton;
    [SerializeField] private Button ExitButton;
    [SerializeField] private Button RoomListButton;
    
    [SerializeField] private Menu menu;
    
    public Menu Menu => menu;
    
    public event Action OnClickCreateRoom;
    public event Action OnClickRandomRoom;
    public event Action OnClickJoinById;
    public event Action OnClickExit;
    public event Action OnClickRoomList;

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

    private void Start()
    {
        CreateRoomButton.onClick.AddListener(CreateRoom);
        RandomRoomButton.onClick.AddListener(RandomRoom);
        JoinByIdButton.onClick.AddListener(JoinById);
        ExitButton.onClick.AddListener(Exit);
        RoomListButton.onClick.AddListener(RoomList);
    }
}
