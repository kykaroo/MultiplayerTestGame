using System.Collections;
using UnityEngine;

namespace Game.ItemSystem.NewSystem
{
    [RequireComponent(typeof(Rigidbody))]
    public class Bullet : MonoBehaviour
    {
        private Rigidbody _rigidbody;

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
        }

        public void Spawn(Vector3 spawnForce)
        {
            SpawnLocation = transform.position;
            transform.forward = spawnForce.normalized;
            _rigidbody.AddForce(spawnForce);
            StartCoroutine(DelayedDisable(_delayedDisableTime));
        }

        private IEnumerator DelayedDisable(float time)
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
