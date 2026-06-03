using Game.Managers;
using Game.Units;
using UnityEngine;

namespace Game.Controllers 
{
    public class UnitVisualController : MonoBehaviour
    {
        [SerializeField]
        private Unit _unit;

        [SerializeField]
        private VisualManager _manager;

        private void OnEnable()
        {
            _unit.OnDied += ShowDeathEffect;
            _unit.OnHealthChanged += PlayDamageAnimation;
            _unit.OnShoot += PlayShootVFX;
        }

        private void OnDisable()
        {
            _unit.OnDied -= ShowDeathEffect;
            _unit.OnHealthChanged -= PlayDamageAnimation;
        }

        private void ShowDeathEffect(Unit unit)
        {
            _manager.ShowDeathEffect(unit.transform.position);
        }

        private void PlayDamageAnimation(int health, int _)
        {
            if (health > 0)
                _manager.AnimateDamage();
        }

        private void PlayShootVFX(Transform _)
        {
            _manager.PlayFireVFX();
        }
    }
}