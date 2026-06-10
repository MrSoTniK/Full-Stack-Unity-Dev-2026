using UnityEngine;

namespace Game.GUI
{
    public class GameHUDScoreController : MonoBehaviour
    {
        [SerializeField]
        private GameHUD _gameHUD;

        [SerializeField]
        private Game.Enemy.EnemiesManager _enemiesManager;

        private void OnEnable()
        {
            _enemiesManager.OnEnemyDespawned += IncreaseScore;
        }

        private void OnDisable()
        {
            _enemiesManager.OnEnemyDespawned -= IncreaseScore;
        }

        private void IncreaseScore(Game.Enemy.EnemyAgent _)
        {
            _gameHUD.IncreaseScore();
        }
    }
}