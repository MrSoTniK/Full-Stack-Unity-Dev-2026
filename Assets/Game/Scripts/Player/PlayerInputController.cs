using UnityEngine;

namespace Game 
{
    public class PlayerInputController : MonoBehaviour
    {
        [SerializeField]
        private Unit _player;

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                _player.Shoot(_player.transform.up);

            float dx = Input.GetAxisRaw("Horizontal");
            float dy = Input.GetAxisRaw("Vertical");

            var moveDirection = new Vector2(dx, dy);
            if (moveDirection == Vector2.zero) return;

            _player.Move(Time.fixedDeltaTime, moveDirection);
        }
    }
}