using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class JoinByIDMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField RoomNameInputField;

    [SerializeField] private Button JoinButton;
    [SerializeField] private Button BackButton;
    
    [SerializeField] private Menu menu;
    
    public string roomName => RoomNameInputField.text;
    public Menu Menu => menu;

    public event Action OnClickJoin;
    public event Action OnClickBack;

    protected virtual void Join()
    {
        OnClickJoin?.Invoke();
    }

    protected virtual void Back()
    {
        OnClickBack?.Invoke();
    }

    private void Start()
    {
        JoinButton.onClick.AddListener(Join);
        BackButton.onClick.AddListener(Back);
    }
}
