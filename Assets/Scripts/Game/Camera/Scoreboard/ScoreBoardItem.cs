using Photon.Pun;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Game.Player
{
    public class ScoreBoardItem : MonoBehaviourPunCallbacks
    {
        public TextMeshProUGUI playerNameText;
        public TextMeshProUGUI killsText;
        public TextMeshProUGUI deathText;

        private Photon.Realtime.Player _player;

        public void Initialize(Photon.Realtime.Player player)
        {
            _player = player;
     
            playerNameText.text = player.NickName;
            UpdateStats();
        }

        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changeProps)
        {
            if (!Equals(targetPlayer, _player)) return;
            if (changeProps.ContainsKey("kills") || changeProps.ContainsKey("deaths"))
            {
                UpdateStats();
            }
        }

        private void UpdateStats()
        {
            if(_player.CustomProperties.TryGetValue("kills", out var kills))
                killsText.text = kills!.ToString();
        
            if(_player.CustomProperties.TryGetValue("deaths", out var deaths))
                deathText.text = deaths.ToString();
        }
    }
}
