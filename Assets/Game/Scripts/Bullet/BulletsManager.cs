using Modules.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game 
{
    public class BulletsManager : MonoBehaviour
    {
        public Action<Bullet> OnBulletSpawned;
        public Action<Bullet> OnBulletMoved;

        [SerializeField]
        private GameObjectPool _pool;

        [SerializeField]
        private List<Bullet> _activeElements;

        [SerializeField]
        private TransformBounds _levelBounds;

        private void FixedUpdate()
        {
            for (int index = _activeElements.Count - 1; index>= 0; index--) 
            {
                var bullet = _activeElements[index];

                if (!_levelBounds.InBounds(bullet.transform.position)) 
                {
                    Despawn(bullet);
                    continue;
                }

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
            _activeElements.Add(bullet);
            bullet.gameObject.SetActive(true);
            bullet.OnDispose += Despawn;
            OnBulletSpawned?.Invoke(bullet);
        }

        public void Despawn(Bullet bullet) 
        {
            if (!_activeElements.Contains(bullet)) return;

            bullet.OnDispose -= Despawn;
            _pool.Release(bullet.gameObject);
            _activeElements.Remove(bullet);
        }
    }
}
