using Game.Components.Movement;
using System;
using UnityEngine;

namespace Game.Units 
{
    public class Unit : MonoBehaviour
    {
        public Action<Unit> OnDied;
        public Action<int, int> OnHealthChanged;
        public Action<Transform> OnShoot;

        [Header("Health")]
        [SerializeField]
        private int _startingHealth;

        [SerializeField]
        private int _currentHealth;

        [Header("Shoot")]
        [SerializeField]
        private float _fireCooldown = 1.25f;

        [SerializeField]
        private Transform _firePoint;

        [Header("Movement")]
        [SerializeField]
        private MovementComponentAbstract _movementComponent;

        private float _fireTime;

        private void OnEnable()
        {
            _currentHealth = _startingHealth;
        }

        public void Move(float fixedDeltaTime, Vector2 direction) => _movementComponent.Move(fixedDeltaTime, direction);

        public void Shoot() 
        {
            float time = Time.time;
            if (time - _fireTime >= _fireCooldown)
            {
                OnShoot?.Invoke(_firePoint);
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