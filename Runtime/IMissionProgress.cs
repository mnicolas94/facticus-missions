using System;
using System.Globalization;
using UnityEngine;

namespace Missions
{
    public interface IMissionProgress
    {
        float GetCurrentProgress();
        float GetMaxProgress();
        Action OnProgressChanged { get; set; }

        string FormatProgressValues(float value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
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