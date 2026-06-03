using Game.Managers;
using Game.Units;
using UnityEngine;

namespace Game.Controllers 
{
    public class PlayerInputController : MonoBehaviour
    {
        [SerializeField]
        private Unit _player;

        [SerializeField]
        private VisualManager _visualManager;

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                _player.Shoot();

            float dx = Input.GetAxisRaw("Horizontal");
            float dy = Input.GetAxisRaw("Vertical");

            var moveDirection = new Vector2(dx, dy);
            if (moveDirection == Vector2.zero) return;

            _player.Move(Time.fixedDeltaTime, moveDirection);
            _visualManager.SetMoveDirection(moveDirection);
        }
    }
}