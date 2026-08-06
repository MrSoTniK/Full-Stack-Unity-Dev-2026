using UnityEngine;

namespace Game 
{
    public class PlayerGameOverController : MonoBehaviour
    {
        [SerializeField] private GameObject[] _objectsToTurnOff;

        [SerializeField]
        private Unit _player;

        private void OnEnable()
        {
            _player.OnDied += DisableObjects;
        }

        private void OnDisable()
        {
            _player.OnDied -= DisableObjects;
        }

        private void DisableObjects(Unit _)
        {
            _player.gameObject.SetActive(false);

            foreach (var obj in _objectsToTurnOff) 
            {
                obj.SetActive(false);
            }
        }
    }
}