using System;
using Game.Player;
using Photon.Pun;
using UnityEngine;

public class CameraHolder : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    private PhotonView _photonView;
    private Animator handsAnimator;
    private static readonly int Reload = Animator.StringToHash("Reload");
    private static readonly int IsDead = Animator.StringToHash("IsDead");

    public event Action OnReloadEnded;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        handsAnimator = GetComponent<Animator>();
    }

    public void PlayAnimation()
    {
        _photonView.RPC(nameof(RPC_PlayAnimation), RpcTarget.All);
    }

    public void Dead()
    {
        _photonView.RPC(nameof(RPC_Dead), RpcTarget.All);
    }

    public void Respawn()
    {
        _photonView.RPC(nameof(RPC_Respawn), RpcTarget.All);
    }

    private void EndReload()
    {
        OnReloadEnded?.Invoke();
    }

    [PunRPC]
    void RPC_PlayAnimation()
    {
        handsAnimator.SetTrigger(Reload);
    }
    
    [PunRPC]
    void RPC_Dead()
    {
        handsAnimator.SetBool(IsDead, true);
    }
    
    [PunRPC]
    void RPC_Respawn()
    {
        handsAnimator.SetBool(IsDead, false);
    }
}
