using UnityEngine;

namespace Game 
{
    public class EnemyAgent : MonoBehaviour
    {
        [SerializeField]
        private Unit _self;

        [SerializeField]
        private float _stoppingDistance = 0.25f;

        private Vector2 _destination;
        private Transform _target;

        public void Init(Vector2 position, Vector2 destination, Transform target)
        {
            transform.position = position;
            _destination = destination;
            _target = target;
        }

        public void ManageAI(float fixedDeltaTime) 
        {
            switch (IsDestinationReached(out var moveDirection))
            {
                case true:
                    _self.Move(fixedDeltaTime, moveDirection);
                    break;
                case false:
                    Shoot();
                    break;
            }
        }

        private void Shoot() 
        {
            Vector2 direction = (_target.position - transform.position).normalized;
            _self.Shoot(direction);
        }

        private bool IsDestinationReached(out Vector2 moveDirection)
        {
            Vector2 distance = _destination - (Vector2)this.transform.position;
            var isNotReached = distance.sqrMagnitude > _stoppingDistance * _stoppingDistance;
            moveDirection = isNotReached ? distance.normalized : Vector3.zero;
            return isNotReached;
        }
    }
}