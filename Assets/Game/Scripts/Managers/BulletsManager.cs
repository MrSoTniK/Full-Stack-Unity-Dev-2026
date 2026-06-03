using Game.Bullets;
using Game.Pools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Managers 
{
    public class BulletsManager : MonoBehaviour
    {
        public Action<Bullet> OnBulletSpawned;
        public Action<Bullet> OnBulletMoved;

        [SerializeField]
        private MonoPool _pool;

        [SerializeField]
        private List<Bullet> _activeElements;

        private List<Bullet> _cashedElementsToAdd = new();
        private List<Bullet> _cashedElementsToRemove = new();

        private void FixedUpdate()
        {
            TryToRemoveCahedElementsFromActive();
            TryToAddCahedElementsToActive();

            foreach (var bullet in _activeElements) 
            {
                bullet.Move(Time.fixedDeltaTime);
                OnBulletMoved?.Invoke(bullet);
            }
        }

        public void Spawn(Vector2 position, Vector2 direction) 
        {
            var bulletGO = _pool.Get();

            var bullet = bulletGO.GetComponent<Bullet>();
            if (bullet == null) return;

            bullet.Init(position, direction);
            _cashedElementsToAdd.Add(bullet);
            bullet.gameObject.SetActive(true);
            bullet.OnDamageDealt += Despawn;
            OnBulletSpawned?.Invoke(bullet);
        }

        public void Despawn(Bullet bullet) 
        {
            if (!_activeElements.Contains(bullet)) return;

            bullet.OnDamageDealt -= Despawn;
            _cashedElementsToRemove.Add(bullet);
            _pool.Release(bullet.gameObject);
        }

        private void TryToAddCahedElementsToActive() 
        {
            if (_cashedElementsToAdd.Count == 0) return;

            foreach (var element in _cashedElementsToAdd)
                _activeElements.Add(element);

            _cashedElementsToAdd.Clear();
        }

        private void TryToRemoveCahedElementsFromActive()
        {
            if (_cashedElementsToRemove.Count == 0) return;
            foreach (var element in _cashedElementsToRemove)
                _activeElements.Remove(element);

            _cashedElementsToRemove.Clear();
        }
    }
}
