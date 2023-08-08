using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomMenu : MonoBehaviour
{
    [SerializeField] private Button CreateRoomButton;
    [SerializeField] private Button BackButton;

    [SerializeField] private TMP_InputField RoomNameInputField;
    [SerializeField] private TMP_InputField MaxPlayersInputField;
    
    [SerializeField] private Menu menu;

    public string roomName => RoomNameInputField.text;
    public string maxPlayer => MaxPlayersInputField.text;
    public Menu Menu => menu;

    public event Action OnClickCreateRoom;
    public event Action OnClickBack;

    protected virtual void CreateRoom()
    {
        OnClickCreateRoom?.Invoke();
    }

    protected virtual void Back()
    {
        OnClickBack?.Invoke();
    }

    private void Start()
    {
        CreateRoomButton.onClick.AddListener(CreateRoom);
        BackButton.onClick.AddListener(Back);
    }
}
