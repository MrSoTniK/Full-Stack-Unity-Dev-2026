using System;
using UnityEngine;

namespace Game
{
    public class Bullet : MonoBehaviour
    {
        public Action<Bullet> OnDispose;

        [SerializeField]
        private Vector2 _direction;

        [SerializeField]
        private int _damage;

        [SerializeField]
        private float _speed;

        public void Init(Vector2 position, Vector2 direction) 
        {
            _direction = direction;
            transform.position = position;
            transform.rotation = Quaternion.LookRotation(_direction, Vector3.forward);
        }

        private void OnTriggerEnter2D(Collider2D other) 
        {
            if (!other.TryGetComponent(out Unit unit) || unit.gameObject.layer == gameObject.layer)
                return;

            DealDamage(unit);
        }

        public void Move(float fixedDeltaTime)
        {
            Vector3 moveStep = _direction * _speed * fixedDeltaTime;
            transform.position += moveStep;
        }

        private void DealDamage(Unit unit)
        {
            if (_damage <= 0) return;
            unit.TakeDamage(_damage);
            gameObject.SetActive(false);
            OnDispose?.Invoke(this);
        }
    }
}
