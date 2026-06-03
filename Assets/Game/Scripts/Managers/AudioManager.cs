using UnityEngine;

namespace Game.Managers 
{
    public class AudioManager : MonoBehaviour
    {
        [Header("SFX")]
        [SerializeField]
        private AudioClip _fireSFX;

        [SerializeField]
        private AudioClip _damageSFX;

        [SerializeField]
        private AudioSource _audioSource;

        public void PlayFireSFX() 
        {
            _audioSource.PlayOneShot(_fireSFX);
        }

        public void PlayDamageSFX() 
        {
            _audioSource.PlayOneShot(_damageSFX);
        }
    }
}