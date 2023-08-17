using System;
using Lobby.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby.Menus
{
    public class RoomListMenu : MonoBehaviour
    {
        [SerializeField] private Button backButton;
        [SerializeField] private Transform roomListContent;
    
        private RoomListItem _roomListEntryPrefab;

        public event Action OnClickBackButton;

        protected virtual void Back()
        {
            OnClickBackButton?.Invoke();
        }

        private void Start()
        {
            backButton.onClick.AddListener(Back);
        }

        private void Awake()
        {
            _roomListEntryPrefab = Resources.Load<RoomListItem>("Lobby/ItemForListPrefabs/RoomListItemPrefab");
        }

        public RoomListItem CreateRoomListItem()
        {
            var roomListEntryGameObject = Instantiate(_roomListEntryPrefab, roomListContent, true);
            roomListEntryGameObject.transform.localScale = Vector3.one;
            roomListEntryGameObject.transform.localPosition = roomListContent.position;
            return roomListEntryGameObject;
        }
    }
}
