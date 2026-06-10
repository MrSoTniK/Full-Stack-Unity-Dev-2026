using Game.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Enemy 
{
    public class EnemiesManager : MonoBehaviour
    {
        public Action<EnemyAgent> OnEnemySpawned;
        public Action<EnemyAgent> OnEnemyDespawned;

        [Header("Spawn")]
        [SerializeField]
        private float _minSpawnCooldown = 2;

        [SerializeField]
        private float _maxSpawnCooldown = 3;

        [SerializeField]
        private Transform _target;

        [Header("Pools")]
        [SerializeField]
        private GameObjectPool _enemiesPool;

        [SerializeField]
        private TransformsPool _attackPosPool;

        [SerializeField]
        private TransformsPool _spawnPosPool;

        [Header("Units")]
        [SerializeField]
        private List<EnemyAgent> _activeElements;

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
            var enemyAgent = enemyGO.GetComponent<EnemyAgent>();
            if (enemyAgent == null) return;

            var position = _spawnPosPool.NextPos();
            var destination = _attackPosPool.NextPos();
            enemyAgent.Init(position, destination, _target);

            var enemy = enemyGO.GetComponent<Unit.Unit>();
            enemy.OnDied += Despawn;
            enemy.gameObject.SetActive(true);

            _activeElements.Add(enemyAgent);
            OnEnemySpawned?.Invoke(enemyAgent);
        }

        private void ResetSpawnCooldown()
        {
            _spawnCooldown = UnityEngine.Random.Range(_minSpawnCooldown, _maxSpawnCooldown);
            _spawnTime = UnityEngine.Time.fixedTime;
        }

        private void Despawn(Unit.Unit unit)
        {
            var enemyAgent = unit.GetComponent<EnemyAgent>();
            if (enemyAgent == null) return;

            this.StartCoroutine(DespawnInNextFrame(enemyAgent, unit));
        }

        private IEnumerator DespawnInNextFrame(EnemyAgent enemyAgent, Unit.Unit unit)
        {
            yield return null;
            _activeElements.Remove(enemyAgent);
            _enemiesPool.Release(enemyAgent.gameObject);
            unit.OnDied -= Despawn;
            OnEnemyDespawned?.Invoke(enemyAgent);
        }
    }
}