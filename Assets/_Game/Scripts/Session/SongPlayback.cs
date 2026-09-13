using System;
using UnityEngine;

namespace DuetCats.Session
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public sealed class SongPlayback : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;

        private AudioClip clip;
        private bool isRunning;
        private float audioStartClock;
        private float songTime;

        public float SongTime { get { return songTime; } }
        public bool IsRunning { get { return isRunning; } }
        public bool HasReachedEnd
        {
            get
            {
                return isRunning && clip != null && songTime >= clip.length;
            }
        }

        public void Configure(AudioClip nextClip)
        {
            if (nextClip == null)
            {
                throw new InvalidOperationException("SongPlayback needs an AudioClip.");
            }

            EnsureAudioSource();
            Stop();

            clip = nextClip;
            audioSource.clip = clip;
            audioSource.playOnAwake = false;
        }

        public void Begin(float preRollStart, float currentClock)
        {
            if (preRollStart > 0f)
            {
                throw new ArgumentOutOfRangeException("preRollStart", "Pre-roll cannot start after song time zero.");
            }

            if (clip == null)
            {
                throw new InvalidOperationException("SongPlayback must be configured before Begin.");
            }

            EnsureAudioSource();
            Stop();

            audioStartClock = currentClock - preRollStart;
            songTime = preRollStart;
            isRunning = true;

            // This runs in the first touch/click path, preserving browser audio permission.
            audioSource.PlayDelayed(-preRollStart);
        }

        public float Tick(float currentClock)
        {
            if (isRunning)
            {
                songTime = currentClock - audioStartClock;
            }

            return songTime;
        }

        public void Stop()
        {
            if (audioSource != null)
            {
                audioSource.Stop();
            }

            isRunning = false;
            songTime = 0f;
        }

        private void Reset()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void EnsureAudioSource()
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }

            if (audioSource == null)
            {
                throw new InvalidOperationException("SongPlayback needs an AudioSource component.");
            }
        }
    }
}
