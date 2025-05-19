using System;
using UnityEngine;
using Utils.Attributes;
using Utils.Serializables;

namespace Missions
{
    /// <summary>
    /// Will regenerate missions in a cyclic way (e.g. daily or weekly)
    /// </summary>
    public class MissionsCyclicRegenerator : MonoBehaviour
    {
        [SerializeField, AutoProperty] private MissionsManager _missionsManager;
        [SerializeField] private float _firstCheckDelay;
        [SerializeField] private float _checkRefreshCooldown;
        [SerializeField] private SerializableTimeSpan _timeInterval;

        private void Start()
        {
            InvokeRepeating(nameof(CheckTimeInterval), _firstCheckDelay, _checkRefreshCooldown);
        }

        private void CheckTimeInterval()
        {
            var now = DateTime.UtcNow;
            var lastRefresh = GetLastRefresh();
            var difference = now - lastRefresh;
            var needRefresh = difference.TotalSeconds >= _timeInterval.ToSeconds();

            if (needRefresh)
            {
                _missionsManager.CurrentMissions.UpdateRefreshTime();
                _missionsManager.ClearMissions();
                _missionsManager.EnsureMaxMission();
                _missionsManager.StartMissions();
            }
        }

        private DateTime GetLastRefresh()
        {
            var serializedTime = _missionsManager.CurrentMissions.LastRefreshTime;
            return DateTime.TryParse(serializedTime, out var parsed) ? parsed : DateTime.MinValue;
        }
    }
}