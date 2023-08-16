using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Game.Player
{
    public class ScoreBoard : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Transform container;
        [SerializeField] private GameObject scoreBoardItemPrefab;
        [SerializeField] private CanvasGroup canvasGroup;

        private Dictionary<Photon.Realtime.Player, ScoreBoardItem> _scoreBoardItems = new();

        private void Start()
        {
            foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
            {
                AddScoreBoardItem(player);
            }
        }

        private void AddScoreBoardItem(Photon.Realtime.Player player)
        {
            ScoreBoardItem item = Instantiate(scoreBoardItemPrefab, container).GetComponent<ScoreBoardItem>();
            item.Initialize(player);
            _scoreBoardItems[player] = item;
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            AddScoreBoardItem(newPlayer);
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            RemoveScoreBoardItem(otherPlayer);
        }

        private void RemoveScoreBoardItem(Photon.Realtime.Player player)
        {
            Destroy(_scoreBoardItems[player].gameObject);
            _scoreBoardItems.Remove(player);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                canvasGroup.alpha = 1;
            }
            else if (Input.GetKeyUp(KeyCode.Tab))
            {
                canvasGroup.alpha = 0;
            }
        }
    }
}
