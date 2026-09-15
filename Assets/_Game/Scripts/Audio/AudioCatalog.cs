using System;
using UnityEngine;

namespace DuetCats.Audio
{
    public enum AudioKey
    {
        SFX_Win = 0,
        SFX_Lose = 1,
        SFX_RainbowNote = 2
    }

    [CreateAssetMenu(fileName = "AudioCatalog", menuName = "Duet Cats/Audio Catalog")]
    public sealed class AudioCatalog : ScriptableObject
    {
        [SerializeField] private AudioEntry[] entries = new AudioEntry[0];

        public bool TryGetClip(AudioKey key, out AudioClip clip)
        {
            if (entries != null)
            {
                for (var index = 0; index < entries.Length; index++)
                {
                    var entry = entries[index];
                    if (entry != null && entry.Key == key)
                    {
                        clip = entry.Clip;
                        return clip != null;
                    }
                }
            }

            clip = null;
            return false;
        }
    }

    [Serializable]
    public sealed class AudioEntry
    {
        [SerializeField] private AudioKey key;
        [SerializeField] private AudioClip clip;

        public AudioKey Key { get { return key; } }
        public AudioClip Clip { get { return clip; } }
    }
}
