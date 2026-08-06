using UnityEngine;

namespace Game 
{
    public class GameHUDController : MonoBehaviour
    {
        [SerializeField]
        private GameHUD _gameHUD;

        [SerializeField]
        private Unit _player;

        [SerializeField]
        private EnemiesManager _enemiesManager;

        private void OnEnable()
        {
            _player.OnHealthChanged += ShowHealthChange;
            _player.OnDied += ShowGameOver;
            _enemiesManager.OnEnemyDespawned += IncreaseScore;
        }

        private void OnDisable()
        {
            _player.OnHealthChanged -= ShowHealthChange;
            _player.OnDied -= ShowGameOver;
            _enemiesManager.OnEnemyDespawned -= IncreaseScore;
        }

        private void IncreaseScore(EnemyAgent _)
        {
            _gameHUD.IncreaseScore();
        }

        private void ShowHealthChange(int health, int maxHealth)
        {
            _gameHUD.SetHealthBarPoints(health, maxHealth);
            _gameHUD.ShakeCamera();
        }

        private void ShowGameOver(Unit _)
        {
            _gameHUD.ShowGameOver();
        }
    }
}