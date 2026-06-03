using Game.Bullets;
using Game.Managers;
using Modules.Utils;
using UnityEngine;

namespace Game.Controllers 
{
    public class LevelBoundsController : MonoBehaviour
    {
        [SerializeField]
        private BulletsManager _bulletsManager;

        [SerializeField]
        private TransformBounds _levelBounds;

        private void OnEnable()
        {
            _bulletsManager.OnBulletMoved += CheckBounds;
        }

        private void OnDisable()
        {
            _bulletsManager.OnBulletMoved -= CheckBounds;
        }

        private void CheckBounds(Bullet bullet)
        {
            if (_levelBounds.InBounds(bullet.transform.position)) return; 

            _bulletsManager.Despawn(bullet);
        }
    }
}