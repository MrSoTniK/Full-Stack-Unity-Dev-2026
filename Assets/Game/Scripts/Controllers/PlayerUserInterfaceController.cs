using Game.Managers;
using Game.Units;
using UnityEngine;

namespace Game.Controllers 
{
    public class PlayerUserInterfaceController : MonoBehaviour
    {
        [SerializeField]
        private UserInterfaceManager _userInterfaceManager;

        [SerializeField]
        private Unit _player;

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
            _userInterfaceManager.SetHealthBarPoints(health, maxHealth);
            _userInterfaceManager.ShakeCamera();
        }

        private void ShowGameOver(Unit _)
        {
            _userInterfaceManager.ShowGameOver();
        }
    }
}