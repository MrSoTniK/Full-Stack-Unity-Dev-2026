using UnityEngine;

namespace Game.Components.Movement
{
    public class RigidbodyMovementComponent : MovementComponentAbstract
    {
        [SerializeField]
        private Rigidbody2D _rigidbody;

        [SerializeField]
        private float _speed;

        public override void Move(float fixedDeltaTime, Vector2 direction)
        {
            Vector2 newPosition = _rigidbody.position + direction * (_speed * fixedDeltaTime);
            _rigidbody.MovePosition(newPosition);
        }
    }
}