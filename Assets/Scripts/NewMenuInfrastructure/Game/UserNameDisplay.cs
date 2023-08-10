using Photon.Pun;
using TMPro;
using UnityEngine;

public class UserNameDisplay : MonoBehaviour
{
    [SerializeField] private PhotonView playerPv;
    [SerializeField] private TextMeshProUGUI text;

    private void Start()
    {
        text.text = playerPv.Owner.NickName;
    }
}
