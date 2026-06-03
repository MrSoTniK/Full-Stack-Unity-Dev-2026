using UnityEngine;

namespace Game.Units 
{
    public class EnemyUnit : EnemyUnitAbstract
    {
        [SerializeField]
        private float _stoppingDistance = 0.25f;

        private Vector2 _destination;

        public override void Init(Vector2 position, Vector2 destination)
        {
            transform.position = position;
            _destination = destination;
        }

        public override  void ManageAI(float fixedDeltaTime) 
        {
            switch (IsDestinationReached(out var moveDirection))
            {
                case true:
                    Move(fixedDeltaTime, moveDirection);
                    break;
                case false:
                    Shoot();
                    break;
            }
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