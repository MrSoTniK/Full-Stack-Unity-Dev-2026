using Game.Bullets;
using Game.Pools;
using System;
using UnityEngine;

namespace Game.Controllers 
{
    public class VfxSpawnContoller : MonoBehaviour
    {
        [SerializeField] private GameObject _prefabVFX;
        [SerializeField] private MonoPool _pool;

        private void OnEnable()
        {
            _pool.OnElementReleased += SpawnVfx;
        }

        private void OnDisable()
        {
            _pool.OnElementReleased -= SpawnVfx;
        }

        private void SpawnVfx(Vector3 pos)
        {
            Instantiate(_prefabVFX, pos, _prefabVFX.transform.rotation);
        }
    }
}
