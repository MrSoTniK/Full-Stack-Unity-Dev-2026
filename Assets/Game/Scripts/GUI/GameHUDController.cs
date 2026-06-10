using UnityEngine;

namespace Game.GUI 
{
    public class GameHUDController : MonoBehaviour
    {
        [SerializeField]
        private GameHUD _gameHUD;

        [SerializeField]
        private Unit.Unit _player;

        private void OnEnable()
        {
            _player.OnHealthChanged += ShowHealthChange;
            _player.OnDied += ShowGameOver;
        }

        private void OnDisable()
        {
            _player.OnHealthChanged -= ShowHealthChange;
            _player.OnDied -= ShowGameOver;
        }

        private void ShowHealthChange(int health, int maxHealth)
        {
            _gameHUD.SetHealthBarPoints(health, maxHealth);
            _gameHUD.ShakeCamera();
        }

        private void ShowGameOver(Unit.Unit _)
        {
            _gameHUD.ShowGameOver();
        }
    }
}