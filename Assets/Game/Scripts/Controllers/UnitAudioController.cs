using Game.Managers;
using Game.Units;
using UnityEngine;

namespace Game.Controllers 
{
    public class UnitAudioController : MonoBehaviour
    {
        [SerializeField]
        private Unit _unit;

        [SerializeField]
        private AudioManager _audioManager;

        private void OnEnable()
        {
            _unit.OnShoot += PlayShootSound;
            _unit.OnHealthChanged += PlayDamageSound;
        }

        private void OnDisable()
        {
            _unit.OnShoot -= PlayShootSound;
            _unit.OnHealthChanged -= PlayDamageSound;
        }

        private void PlayShootSound(Transform _)
        {
            _audioManager.PlayFireSFX();
        }

        private void PlayDamageSound(int _, int __)
        {
            _audioManager.PlayDamageSFX();
        }
    }
}