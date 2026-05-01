using UnityEngine;

namespace Scripts.Misc
{
    public class EffectsManager : MonoBehaviour
    {
        public static EffectsManager Instance { get; private set; }

        [SerializeField] private ParticleSystem blastParticleSystem;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void PlayBlastEffect(Vector3 position, Color color)
        {
            var blastParticles = Instantiate(blastParticleSystem, position, Quaternion.identity);
            var main = blastParticles.main;
            main.startColor = color;
            blastParticles.Play();
        }
    }
}