using UnityEngine;

namespace Game 
{
    public class UnitViewController : MonoBehaviour
    {
        [SerializeField]
        private Unit _unit;

        [SerializeField]
        private UnitView _unitView;

        private void OnEnable()
        {
            _unit.OnDied += ShowDeathEffect;
            _unit.OnHealthChanged += PlayDamageAnimation;
            _unit.OnShoot += PlayShootVFX;
            _unit.OnMove += _unitView.SetMoveDirection;
            _unit.OnShoot += _unitView.PlayShootSound;
            _unit.OnHealthChanged += _unitView.PlayDamageSound;
        }

        private void OnDisable()
        {
            _unit.OnDied -= ShowDeathEffect;
            _unit.OnHealthChanged -= PlayDamageAnimation;
            _unit.OnShoot -= PlayShootVFX;
            _unit.OnMove -= _unitView.SetMoveDirection;
            _unit.OnShoot -= _unitView.PlayShootSound;
            _unit.OnHealthChanged -= _unitView.PlayDamageSound;
        }

        private void ShowDeathEffect(Unit unit)
        {
              _unitView.ShowDeathEffect(unit.transform.position);
        }

        private void PlayDamageAnimation(int health, int _)
        {
            if (health > 0)
                _unitView.AnimateDamage();
        }

        private void PlayShootVFX(Transform _)
        {
            _unitView.PlayFireVFX();
        }
    }
}