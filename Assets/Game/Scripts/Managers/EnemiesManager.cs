using Game.Pools;
using Game.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Managers 
{
    public class EnemiesManager : MonoBehaviour
    {
        public Action<EnemyUnitAbstract> OnEnemySpawned;
        public Action<EnemyUnitAbstract> OnEnemyDespawned;

        [Header("Spawn")]
        [SerializeField]
        private float _minSpawnCooldown = 2;

        [SerializeField]
        private float _maxSpawnCooldown = 3;

        [Header("Pools")]
        [SerializeField]
        private MonoPool _enemiesPool;

        [SerializeField]
        private TransformsPool _attackPosPool;

        [SerializeField]
        private TransformsPool _spawnPosPool;

        [Header("Units")]
        [SerializeField]
        private List<EnemyUnitAbstract> _activeElements;

        private float _spawnCooldown;
        private float _spawnTime;

        private void FixedUpdate()
        {
            TryToSpawnNewEnemy();

            foreach (var enemy in _activeElements) 
            {
                enemy.ManageAI(Time.fixedDeltaTime);
            }        
        }

        private void TryToSpawnNewEnemy() 
        {
            if (Time.fixedTime - _spawnTime < _spawnCooldown)
                return;

            Spawn();
            ResetSpawnCooldown();
        }

        private void Spawn()
        {
            var enemyGO = _enemiesPool.Get();
            var enemy = enemyGO.GetComponent<EnemyUnitAbstract>();
            if (enemy == null) return;

            var position = _spawnPosPool.NextPos();
            var destination = _attackPosPool.NextPos();
            enemy.Init(position, destination);
            enemy.OnDied += Despawn;
            enemy.gameObject.SetActive(true);

            _activeElements.Add(enemy);
            OnEnemySpawned?.Invoke(enemy);
        }

        private void ResetSpawnCooldown()
        {
            _spawnCooldown = UnityEngine.Random.Range(_minSpawnCooldown, _maxSpawnCooldown);
            _spawnTime = UnityEngine.Time.fixedTime;
        }

        private void Despawn(Unit unit)
        {
            var enemy = unit.GetComponent<EnemyUnitAbstract>();
            if (enemy == null) return;

            this.StartCoroutine(DespawnInNextFrame(enemy));
        }

        private IEnumerator DespawnInNextFrame(EnemyUnitAbstract enemy)
        {
            yield return null;
            _activeElements.Remove(enemy);
            _enemiesPool.Release(enemy.gameObject);
            enemy.OnDied -= Despawn;
            OnEnemyDespawned?.Invoke(enemy);
        }
    }
}