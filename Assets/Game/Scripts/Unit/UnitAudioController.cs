using UnityEngine;

namespace Game.Unit 
{
    public class UnitAudioController : MonoBehaviour
    {
        [SerializeField]
        private Unit _unit;

        [Header("SFX")]
        [SerializeField]
        private AudioClip _fireSFX;

        [SerializeField]
        private AudioClip _damageSFX;

        [SerializeField]
        private AudioSource _audioSource;

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
            _audioSource.PlayOneShot(_fireSFX);
        }

        private void PlayDamageSound(int _, int __)
        {
            _audioSource.PlayOneShot(_damageSFX);
        }
    }
}