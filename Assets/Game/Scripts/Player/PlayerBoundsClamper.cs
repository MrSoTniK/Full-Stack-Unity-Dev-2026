using Modules.Utils;
using UnityEngine;

namespace Game.Player
{
    public class PlayerBoundsClamper : MonoBehaviour
    {
        [SerializeField]
        private Transform _player;

        [SerializeField]
        private TransformBounds _playerArea;

        private void LateUpdate()
        {
            this._player.transform.position = _playerArea.ClampInBounds(this.transform.position);
        }
    }
}