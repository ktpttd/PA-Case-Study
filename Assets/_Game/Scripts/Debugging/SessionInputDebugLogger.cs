using DuetCats.Controls;
using DuetCats.Session;
using UnityEngine;

namespace DuetCats.Debugging
{
    [DisallowMultipleComponent]
    public sealed class SessionInputDebugLogger : MonoBehaviour
    {
        [SerializeField] private GameSession gameSession;
        [SerializeField] private CatInput catInput;
        [SerializeField, Min(0.1f)] private float heartbeatSeconds = 1f;

        private GamePhase lastPhase;
        private float nextHeartbeatTime;
        private bool loggedStartup;

        private void Awake()
        {
            if (gameSession == null)
            {
                gameSession = GetComponent<GameSession>();
            }

            if (catInput == null)
            {
                catInput = GetComponent<CatInput>();
            }
        }

        private void Update()
        {
            if (gameSession == null || catInput == null)
            {
                return;
            }

            if (!loggedStartup)
            {
                LogStartup();
                loggedStartup = true;
                lastPhase = gameSession.Phase;
            }

            if (gameSession.Phase != lastPhase)
            {
                Debug.Log("[Duet Cats] Session phase: " + lastPhase + " -> " + gameSession.Phase, this);
                lastPhase = gameSession.Phase;
            }

            if (gameSession.Phase == GamePhase.Playing && Time.unscaledTime >= nextHeartbeatTime)
            {
                Debug.Log(string.Format(
                    "[Duet Cats] songTime={0:F2}; leftX={1:F3}; rightX={2:F3}",
                    gameSession.SongTime,
                    catInput.LeftCatX,
                    catInput.RightCatX), this);
                nextHeartbeatTime = Time.unscaledTime + heartbeatSeconds;
            }
        }

        private void LogStartup()
        {
            if (gameSession.SongContent == null)
            {
                Debug.LogError("[Duet Cats] Session failed to prepare: " + gameSession.PlayerMessage, this);
                return;
            }

            var content = gameSession.SongContent;
            Debug.Log(string.Format(
                "[Duet Cats] Content ready: notes={0}; lanes={1} (left={2}, right={3}); " +
                "first={4:F3}s; last={5:F3}s; score={6}; catX=({7:F3}, {8:F3})",
                content.Notes.Count,
                content.LaneCount,
                content.LeftLaneCount,
                content.RightLaneCount,
                content.FirstHitTime,
                content.LastHitTime,
                content.MaxScore,
                catInput.LeftCatX,
                catInput.RightCatX), this);
        }
    }
}
