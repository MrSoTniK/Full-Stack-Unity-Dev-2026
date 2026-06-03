using Game.Managers;
using Game.Units;
using UnityEngine;

namespace Game.Controllers 
{
    public class EnemiesShootingController : MonoBehaviour
    {
        [SerializeField]
        private EnemiesManager _enemiesManager;

        [SerializeField]
        private BulletsManager _bulletsManager;

        [SerializeField]
        private Transform _target;

        private void OnEnable()
        {
            _enemiesManager.OnEnemySpawned += SetBulletsShooting;
            _enemiesManager.OnEnemyDespawned += UnsetBulletsShooting;
        }

        private void OnDisable()
        {
            _enemiesManager.OnEnemySpawned -= SetBulletsShooting;
            _enemiesManager.OnEnemyDespawned -= UnsetBulletsShooting;
        }

        private void UnsetBulletsShooting(EnemyUnitAbstract enemy)
        {
            enemy.OnShoot -= SpawnBullet;
        }

        private void SetBulletsShooting(EnemyUnitAbstract enemy)
        {
            enemy.OnShoot += SpawnBullet;
        }

        private void SpawnBullet(Transform firePoint)
        {
            Vector2 direction = (_target.position - firePoint.position).normalized;
            _bulletsManager.Spawn(firePoint.position, direction);
        }
    }
}