using System;
using DuetCats.Content;
using UnityEngine;

namespace DuetCats.Session
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SongPlayback))]
    public sealed class GameSession : MonoBehaviour
    {
        private const string DefaultContentErrorMessage = "Unable to load this song.";

        [SerializeField] private SongConfig songConfig;
        [SerializeField] private SongPlayback songPlayback;

        private SongContent songContent;
        private GamePhase phase;
        private GameOutcome outcome;
        private float songTime;
        private string playerMessage;

        public GamePhase Phase { get { return phase; } }
        public GameOutcome Outcome { get { return outcome; } }
        public SongConfig SongConfig { get { return songConfig; } }
        public SongContent SongContent { get { return songContent; } }
        public float SongTime { get { return songTime; } }
        public string PlayerMessage { get { return playerMessage; } }

        private void Awake()
        {
            if (songPlayback == null)
            {
                songPlayback = GetComponent<SongPlayback>();
            }

            Prepare();
        }

        private void Update()
        {
            if (phase != GamePhase.Playing)
            {
                return;
            }

            songTime = songPlayback.Tick(Time.unscaledTime);
        }

        public bool TryStartRun()
        {
            if (phase != GamePhase.Ready || songContent == null)
            {
                return false;
            }

            songTime = Mathf.Min(0f, songContent.FirstHitTime - songConfig.FallDuration);
            songPlayback.Begin(songTime, Time.unscaledTime);
            phase = GamePhase.Playing;
            return true;
        }

        public bool TryFinish(GameOutcome nextOutcome)
        {
            if (phase != GamePhase.Playing || nextOutcome == GameOutcome.None)
            {
                return false;
            }

            outcome = nextOutcome;
            phase = GamePhase.Ending;
            return true;
        }

        public bool CompleteEnding()
        {
            if (phase != GamePhase.Ending)
            {
                return false;
            }

            phase = GamePhase.Result;
            return true;
        }

        private void Prepare()
        {
            phase = GamePhase.Preparing;
            outcome = GameOutcome.None;
            songContent = null;
            songTime = 0f;
            playerMessage = string.Empty;

            try
            {
                songContent = SongContentBuilder.Build(songConfig);
                songPlayback.Configure(songConfig.AudioClip);
                phase = GamePhase.Ready;
            }
            catch (Exception exception)
            {
                Debug.LogError(exception.Message, this);
                playerMessage = DefaultContentErrorMessage;
                phase = GamePhase.ContentError;
            }
        }
    }
}
