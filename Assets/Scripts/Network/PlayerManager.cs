using System.IO;
using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private PhotonView PV;

    private GameObject _controller;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void CreateController()
    {
        Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint();
        _controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrafabs", "PlayerController"), spawnPoint.position, spawnPoint.rotation, 0, new object[] {PV.ViewID});
    }

    private void Start()
    {
        if (PV.IsMine)
        {
            CreateController();
        }
    }

    public void Die()
    {
        PhotonNetwork.Destroy(_controller);
        CreateController();
    }
}
