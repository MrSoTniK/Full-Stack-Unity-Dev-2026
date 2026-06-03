using UnityEngine;

namespace Game.Components.Movement 
{
    public abstract class MovementComponentAbstract : MonoBehaviour
    {
        public abstract void Move(float fixedDeltaTime, Vector2 direction);
    }
}