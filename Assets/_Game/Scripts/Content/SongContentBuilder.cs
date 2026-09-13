using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace DuetCats.Content
{
    public sealed class SongContentException : Exception
    {
        public SongContentException(string message) : base(message)
        {
        }
    }

    public static class SongContentBuilder
    {
        public static SongContent Build(SongConfig config)
        {
            if (config == null)
            {
                throw new SongContentException("SongConfig is missing.");
            }

            var errors = new List<string>();
            ValidateConfig(config, errors);

            var scoreByVelocity = BuildScoreLookup(config.ScoreRules, errors);
            var specialByNoteId = BuildSpecialLookup(config.SpecialNotes, errors);
            var rawNotes = ParseChart(config.Chart, errors);
            var runtimeNotes = new List<RuntimeNote>();
            var noteIds = new HashSet<int>();
            var maxLaneIndex = -1;
            var maxScore = 0;

            if (rawNotes != null)
            {
                for (var index = 0; index < rawNotes.Length; index++)
                {
                    var raw = rawNotes[index];
                    var label = "record " + index + " (id " + raw.id + ")";

                    if (raw.id <= 0)
                    {
                        errors.Add(label + ": id must be greater than zero.");
                    }
                    else if (!noteIds.Add(raw.id))
                    {
                        errors.Add(label + ": duplicate id.");
                    }

                    if (!IsFinite(raw.ta) || raw.ta < 0f)
                    {
                        errors.Add(label + ": ta must be finite and non-negative.");
                    }

                    if (!IsFinite(raw.ts) || raw.ts < 0f)
                    {
                        errors.Add(label + ": ts must be finite and non-negative.");
                    }

                    if (!IsFinite(raw.d) || raw.d < 0f)
                    {
                        errors.Add(label + ": d must be finite and non-negative.");
                    }

                    if (raw.pid < 0)
                    {
                        errors.Add(label + ": pid must be non-negative.");
                    }
                    else if (raw.pid > maxLaneIndex)
                    {
                        maxLaneIndex = raw.pid;
                    }

                    int points;
                    if (!scoreByVelocity.TryGetValue(raw.v, out points))
                    {
                        errors.Add(label + ": no score rule for v=" + raw.v + ".");
                        points = 0;
                    }

                    var kind = NoteKind.Normal;
                    SpecialNoteOverride special;
                    if (specialByNoteId.TryGetValue(raw.id, out special))
                    {
                        kind = special.Kind;
                        points = special.Points;
                    }

                    var hitTime = raw.ta + config.ChartOffset;
                    if (!IsFinite(hitTime) || hitTime < 0f)
                    {
                        errors.Add(label + ": mapped hitTime must be finite and non-negative.");
                    }

                    runtimeNotes.Add(new RuntimeNote(raw.id, hitTime, raw.pid, kind, points));
                    maxScore += points;
                }
            }

            foreach (var pair in specialByNoteId)
            {
                if (!noteIds.Contains(pair.Key))
                {
                    errors.Add("Special-note override references missing note id " + pair.Key + ".");
                }
            }

            runtimeNotes.Sort(CompareNotes);

            var laneCount = maxLaneIndex + 1;
            if (laneCount > 0 && config.LeftLaneCount >= laneCount)
            {
                errors.Add("leftLaneCount must leave at least one lane for the right side.");
            }

            if (config.AudioClip != null && runtimeNotes.Count > 0)
            {
                var finalWindowEnd = runtimeNotes[runtimeNotes.Count - 1].HitTime + config.HitTolerance;
                if (finalWindowEnd > config.AudioClip.length + 0.001f)
                {
                    errors.Add("Final note window ends after the audio clip.");
                }
            }

            ThrowIfInvalid(errors);

            return new SongContent(
                runtimeNotes.ToArray(),
                laneCount,
                config.LeftLaneCount,
                maxScore);
        }

        private static void ValidateConfig(SongConfig config, List<string> errors)
        {
            if (config.Chart == null)
            {
                errors.Add("Chart TextAsset is missing.");
            }

            if (config.AudioClip == null)
            {
                errors.Add("AudioClip is missing.");
            }

            if (!IsFinite(config.ChartOffset))
            {
                errors.Add("chartOffset must be finite.");
            }

            if (!IsFinite(config.FallDuration) || config.FallDuration <= 0f)
            {
                errors.Add("fallDuration must be finite and greater than zero.");
            }

            if (!IsFinite(config.HitTolerance) || config.HitTolerance < 0f)
            {
                errors.Add("hitTolerance must be finite and non-negative.");
            }

            if (!IsFinite(config.CatchDistance) || config.CatchDistance <= 0f)
            {
                errors.Add("catchDistance must be finite and greater than zero.");
            }

            if (config.LeftLaneCount <= 0)
            {
                errors.Add("leftLaneCount must be greater than zero.");
            }

        }

        private static Dictionary<int, int> BuildScoreLookup(
            VelocityScoreRule[] rules,
            List<string> errors)
        {
            var result = new Dictionary<int, int>();

            if (rules == null || rules.Length == 0)
            {
                errors.Add("At least one velocity score rule is required.");
                return result;
            }

            for (var index = 0; index < rules.Length; index++)
            {
                var rule = rules[index];
                if (rule == null)
                {
                    errors.Add("Score rule " + index + " is null.");
                    continue;
                }

                if (rule.Points <= 0)
                {
                    errors.Add("Score rule for velocity " + rule.Velocity + " must award points.");
                }

                if (result.ContainsKey(rule.Velocity))
                {
                    errors.Add("Duplicate score rule for velocity " + rule.Velocity + ".");
                    continue;
                }

                result.Add(rule.Velocity, rule.Points);
            }

            return result;
        }

        private static Dictionary<int, SpecialNoteOverride> BuildSpecialLookup(
            SpecialNoteOverride[] overrides,
            List<string> errors)
        {
            var result = new Dictionary<int, SpecialNoteOverride>();

            if (overrides == null)
            {
                return result;
            }

            for (var index = 0; index < overrides.Length; index++)
            {
                var special = overrides[index];
                if (special == null)
                {
                    errors.Add("Special-note override " + index + " is null.");
                    continue;
                }

                if (special.NoteId <= 0)
                {
                    errors.Add("Special-note override " + index + " has an invalid note id.");
                    continue;
                }

                if (special.Points <= 0)
                {
                    errors.Add("Special-note override for id " + special.NoteId + " must award points.");
                }

                if (result.ContainsKey(special.NoteId))
                {
                    errors.Add("Duplicate special-note override for id " + special.NoteId + ".");
                    continue;
                }

                result.Add(special.NoteId, special);
            }

            return result;
        }

        private static RawMidiNote[] ParseChart(TextAsset chart, List<string> errors)
        {
            if (chart == null)
            {
                return null;
            }

            var json = chart.text == null ? string.Empty : chart.text.Trim();
            if (json.Length > 0 && json[0] == '\uFEFF')
            {
                json = json.Substring(1);
            }

            if (json.Length < 2 || json[0] != '[' || json[json.Length - 1] != ']')
            {
                errors.Add("Chart root must be a JSON array.");
                return null;
            }

            try
            {
                var notes = JsonConvert.DeserializeObject<RawMidiNote[]>(json);
                if (notes == null || notes.Length == 0)
                {
                    errors.Add("Chart must contain at least one note.");
                    return null;
                }

                return notes;
            }
            catch (Exception exception)
            {
                errors.Add("Chart JSON could not be parsed: " + exception.Message);
                return null;
            }
        }

        private static int CompareNotes(RuntimeNote left, RuntimeNote right)
        {
            var timeOrder = left.HitTime.CompareTo(right.HitTime);
            return timeOrder != 0 ? timeOrder : left.Id.CompareTo(right.Id);
        }

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }

        private static void ThrowIfInvalid(List<string> errors)
        {
            if (errors.Count > 0)
            {
                throw new SongContentException(string.Join("\n", errors.ToArray()));
            }
        }

        [Serializable]
        private sealed class RawMidiNote
        {
            public int id = 0;
            public int n = 0;
            public float ta = 0f;
            public float ts = 0f;
            public float d = 0f;
            public int v = 0;
            public int pid = 0;
        }
    }
}
