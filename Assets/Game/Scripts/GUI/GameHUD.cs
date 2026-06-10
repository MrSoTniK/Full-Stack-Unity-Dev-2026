using Modules.UI;
using Modules.Utils;
using UnityEngine;

namespace Game.GUI 
{
    public class GameHUD : MonoBehaviour
    {
        [SerializeField]
        private CameraShaker _cameraShaker;

        [SerializeField]
        private GameOverView _gameOverView;

        [SerializeField]
        private HealthView _healthView;

        [SerializeField]
        private ScoreView _scoreView;

        private int _destroyedEnemies;

        private void Awake()
        {
            _destroyedEnemies = 0;
            _scoreView.SetValue(_destroyedEnemies);
        }

        public void IncreaseScore() 
        {
            _destroyedEnemies++;
            _scoreView.SetValue(_destroyedEnemies);
        }

        public void ShowGameOver() 
        {
            _gameOverView.Show();
        }

        public void SetHealthBarPoints(int health, int maxHealth) 
        {
            _healthView.SetHealth(health, maxHealth);
        }

        public void ShakeCamera() 
        {
            _cameraShaker.Shake();
        }
    }
}