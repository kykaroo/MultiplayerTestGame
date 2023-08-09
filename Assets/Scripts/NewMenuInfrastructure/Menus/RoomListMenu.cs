using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListMenu : MonoBehaviour
{
    [SerializeField] private Button BackButton;

    [SerializeField] private GameObject RoomListContent;
    
    [SerializeField] private Menu menu;
    
    private GameObject roomListEntryPrefab;

    public GameObject roomListContent => RoomListContent;
    public GameObject RoomListEntryPrefab => roomListEntryPrefab;
    public Menu Menu => menu;

    public event Action OnClickBackButton;

    protected virtual void Back()
    {
        OnClickBackButton?.Invoke();
    }

    private void Start()
    {
        BackButton.onClick.AddListener(Back);
    }

    private void Awake()
    {
        roomListEntryPrefab = Resources.Load<GameObject>("ItemForListPrefabs/RoomListItemPrefab");
    }
}
