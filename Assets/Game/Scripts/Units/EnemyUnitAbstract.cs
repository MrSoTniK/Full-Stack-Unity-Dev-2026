using UnityEngine;

namespace Game.Units
{
    public abstract class EnemyUnitAbstract : Unit
    {
        public abstract void Init(Vector2 position, Vector2 destination);

        public abstract void ManageAI(float fixedDeltaTime);
    }
}