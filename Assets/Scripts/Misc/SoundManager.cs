using UnityEngine;

namespace Scripts.Misc
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        private AudioSource audioSource;

        [SerializeField] private AudioClip shootSound;

        [SerializeField] private AudioClip warpSound;

        [SerializeField] private AudioClip inRestingAreaSound;

        [SerializeField] private AudioClip gameWinSound;
        
        [SerializeField] private AudioClip gameLoseSound;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void PlayShootSound()
        {
            if (audioSource != null && shootSound != null)
            {
                audioSource.PlayOneShot(shootSound, 1.0f);
            }
        }

        public void PlayWarpSound()
        {
            if (audioSource != null && warpSound != null)
            {
                audioSource.PlayOneShot(warpSound, 1.0f);
            }
        }

        public void PlayInRestingAreaSound()
        {
            if (audioSource != null && inRestingAreaSound != null)
            {
                audioSource.PlayOneShot(inRestingAreaSound, 1.0f);
            }
        }

        public void PlayGameWinSound()
        {
            if (audioSource != null && gameWinSound != null)
            {
                audioSource.PlayOneShot(gameWinSound, 1.0f);
            }
        }

        public void PlayGameLoseSound()
        {
            if (audioSource != null && gameLoseSound != null)
            {
                audioSource.PlayOneShot(gameLoseSound, 1.0f);
            }
        }
    }
}
