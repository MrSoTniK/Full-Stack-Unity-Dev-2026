using UnityEngine;

namespace Game 
{
    public class PoolVfxSpawnContoller : MonoBehaviour
    {
        [SerializeField] private GameObject _prefabVFX;
        [SerializeField] private GameObjectPool _pool;

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
