using Game.Managers;
using Game.Units;
using UnityEngine;

namespace Game.Controllers 
{
    public class PlayerShootingController : MonoBehaviour
    {
        [SerializeField]
        private Unit _player;

        [SerializeField]
        private BulletsManager _bulletsManager;

        private void OnEnable() 
        {
            _player.OnShoot += SpawnBullet;
        }

        private void OnDisable()
        {
            _player.OnShoot -= SpawnBullet;
        }

        private void SpawnBullet(Transform firePointPos)
        {
            _bulletsManager.Spawn(firePointPos.position, firePointPos.up);
        }
    }
}