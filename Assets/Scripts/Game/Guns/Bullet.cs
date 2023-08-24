using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace Game.Guns
{
    [RequireComponent(typeof(Rigidbody))]
    public class Bullet : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        private PhotonView _photonView;

        [field: SerializeField]
        public Vector3 SpawnLocation
        {
            get;
            private set;
        }

        public float _delayedDisableTime = 2f;

        public delegate void CollisionEvent(Bullet bullet, Collision collision);
        public event CollisionEvent OnCollision;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _photonView = GetComponent<PhotonView>();
        }

        public void Spawn(Vector3 spawnForce)
        {
            _photonView.RPC(nameof(RPC_Spawn), RpcTarget.All, spawnForce);
        }

        [PunRPC]
        void RPC_Spawn(Vector3 spawnForce)
        {
            SpawnLocation = transform.position;
            transform.forward = spawnForce.normalized;
            _rigidbody.AddForce(spawnForce);
             StartCoroutine(DelayedDestroy(_delayedDisableTime));
        }

        private IEnumerator DelayedDestroy(float time)
        {
            yield return new WaitForSeconds(time);
            OnCollisionEnter(null);
        }

        private void OnCollisionEnter(Collision other)
        {
            OnCollision?.Invoke(this, other);
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            OnCollision = null;
        }
    }
}
