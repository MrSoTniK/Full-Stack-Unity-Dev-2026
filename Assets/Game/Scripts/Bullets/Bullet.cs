using Game.Units;
using System;
using UnityEngine;

namespace Game.Bullets
{
    public class Bullet : MonoBehaviour
    {
        public Action<Bullet> OnDamageDealt;

        [SerializeField]
        private Vector2 _direction;

        [SerializeField]
        private int _damage;

        [SerializeField]
        private float _speed;

        [SerializeField]
        private GameObject _vfx;

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

        public void DealDamage(Unit unit)
        {
            if (_damage <= 0) return;
            unit.TakeDamage(_damage);
            gameObject.SetActive(false);
            OnDamageDealt?.Invoke(this);
        }
    }
}
