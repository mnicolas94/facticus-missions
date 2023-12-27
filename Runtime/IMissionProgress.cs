using System;
using UnityEngine;

namespace Missions
{
    public interface IMissionProgress
    {
        float GetCurrentProgress();
        float GetMaxProgress();
        void AddProgressChangedCallback(Action callback);
    }

    public static class IMissionProgressExtensions
    {
        public static float GetNormalizedProgress(this IMissionProgress missionProgress)
        {
            var current = missionProgress.GetCurrentProgress();
            var max = missionProgress.GetMaxProgress();
            var normalized = current / max;
            normalized = Mathf.Clamp01(normalized);
            return normalized;
        }
    }
}