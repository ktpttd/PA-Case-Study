using UnityEngine;

namespace DuetCats.Audio
{
    [DefaultExecutionOrder(-1000)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public sealed class AudioManagerBootstrap : MonoBehaviour
    {
        [SerializeField] private AudioCatalog catalog;
        [SerializeField] private AudioSource soundEffectSource;

        private void Awake()
        {
            if (soundEffectSource == null)
            {
                soundEffectSource = GetComponent<AudioSource>();
            }

            AudioManager.Initialize(catalog, soundEffectSource);
        }

        private void OnDestroy()
        {
            AudioManager.Clear(soundEffectSource);
        }
    }
}
