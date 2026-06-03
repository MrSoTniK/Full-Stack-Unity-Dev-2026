using Game.Managers;
using Game.Units;
using UnityEngine;

namespace Game.Controllers 
{
    public class PlayerScoreController : MonoBehaviour
    {
        [SerializeField]
        private UserInterfaceManager _userInterfaceManager;

        [SerializeField]
        private EnemiesManager _enemiesManager;

        private void OnEnable()
        {
            _enemiesManager.OnEnemyDespawned += IncreaseScore;
        }

        private void OnDisable()
        {
            _enemiesManager.OnEnemyDespawned -= IncreaseScore;
        }

        private void IncreaseScore(EnemyUnitAbstract _)
        {
            _userInterfaceManager.IncreaseScore();
        }
    }
}