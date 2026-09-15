using UnityEngine;

namespace DuetCats.Audio
{
    public static class AudioManager
    {
        private static AudioCatalog catalog;
        private static AudioSource soundEffectSource;

        public static void Initialize(AudioCatalog nextCatalog, AudioSource nextSoundEffectSource)
        {
            catalog = nextCatalog;
            soundEffectSource = nextSoundEffectSource;
        }

        public static void Clear(AudioSource source)
        {
            if (soundEffectSource != source)
            {
                return;
            }

            catalog = null;
            soundEffectSource = null;
        }

        public static void Play(AudioKey key, float volume = 1f)
        {
            if (catalog == null)
            {
                Debug.LogError("AudioManager needs an AudioCatalog.");
                return;
            }

            if (soundEffectSource == null)
            {
                Debug.LogError("AudioManager needs an AudioSource.");
                return;
            }

            AudioClip clip;
            if (!catalog.TryGetClip(key, out clip))
            {
                Debug.LogWarning("AudioCatalog is missing a clip for " + key + ".");
                return;
            }

            soundEffectSource.PlayOneShot(clip, volume);
        }
    }
}
