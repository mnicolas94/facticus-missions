using System;
using UnityEngine;
using Utils.Attributes;
using Utils.Serializables;

namespace Missions
{
    /// <summary>
    /// Will refresh missions in a cyclic way (e.g. daily or weekly)
    /// </summary>
    public class MissionsCyclicRefresher : MonoBehaviour
    {
        [SerializeField, AutoProperty] private MissionsManager _missionsManager;
        [SerializeField] private float _firstCheckDelay;
        [SerializeField] private float _checkRefreshCooldown;

        private void Start()
        {
            InvokeRepeating(nameof(CheckTimeInterval), _firstCheckDelay, _checkRefreshCooldown);
        }

        private void CheckTimeInterval()
        {
            var secondsToNextRefresh = MissionsPoolData.SecondsToNextRefresh(_missionsManager.MissionsPool,
                _missionsManager.CurrentMissions);
            var needRefresh = secondsToNextRefresh <= 0;

            if (needRefresh)
            {
                _missionsManager.CurrentMissions.UpdateRefreshTime();
                _missionsManager.ClearMissions();
                _missionsManager.EnsureMaxMission();
                _missionsManager.StartMissions();
            }
        }
    }
}