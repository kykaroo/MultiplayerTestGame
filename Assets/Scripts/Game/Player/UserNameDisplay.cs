using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Player
{
    public class UserNameDisplay : MonoBehaviour
    { 
        [SerializeField] private PhotonView _photonView;
        [SerializeField] private TextMeshProUGUI text;

        private void Start()
        {
            text.text = _photonView.Owner.NickName;
        }
    }
}
