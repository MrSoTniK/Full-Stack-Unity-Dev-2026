using System;
using UnityEngine;

namespace Game 
{
    public class Unit : MonoBehaviour
    {
        public Action<Unit> OnDied;
        public Action<int, int> OnHealthChanged;
        public Action<Transform> OnShoot;
        public Action<Vector2> OnMove;

        [Header("Health")]
        [SerializeField]
        private int _startingHealth;

        [SerializeField]
        private int _currentHealth;

        [Header("Shoot")]
        [SerializeField]
        private float _fireCooldown = 1.25f;

        [SerializeField]
        private BulletsManager _bulletsManager;

        [SerializeField]
        private Transform _firePoint;

        [Header("Movement")]
        [SerializeField]
        private Rigidbody2D _rigidbody;

        [SerializeField]
        private float _speed;

        private float _fireTime;

        private void OnEnable()
        {
            _currentHealth = _startingHealth;
        }

        public void Move(float fixedDeltaTime, Vector2 direction)
        {
            Vector2 newPosition = _rigidbody.position + direction * (_speed * fixedDeltaTime);
            _rigidbody.MovePosition(newPosition);
            OnMove?.Invoke(direction);
        }

        public void Shoot(Vector2 direction) 
        {
            float time = Time.time;
            if (time - _fireTime >= _fireCooldown)
            {
                _bulletsManager.Spawn(_firePoint.position, direction);
                OnShoot?.Invoke(_firePoint.transform);
                _fireTime = time;
            }
        }

        public void TakeDamage(int damage) 
        {
            _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, _startingHealth);
            OnHealthChanged?.Invoke(_currentHealth, _startingHealth);

            if (_currentHealth <= 0)
            {
                OnDied?.Invoke(this);
                gameObject.SetActive(false);
            }
        }
    }
}