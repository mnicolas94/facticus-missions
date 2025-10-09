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

        private async void Start()
        {
            await Awaitable.WaitForSecondsAsync(_firstCheckDelay, destroyCancellationToken);
            CheckTimeInterval();
        }

        private async void CheckTimeInterval()
        {
            var secondsToNextRefresh = MissionsPoolData.SecondsToNextRefresh(_missionsManager.MissionsPool,
                _missionsManager.CurrentMissions);

            await Awaitable.WaitForSecondsAsync(secondsToNextRefresh, destroyCancellationToken);

            if (destroyCancellationToken.IsCancellationRequested) return;
            
            Refresh();
            CheckTimeInterval();
        }

        [ContextMenu(nameof(Refresh))]
        public void Refresh()
        {
            _missionsManager.CurrentMissions.UpdateRefreshTime();
            _missionsManager.ClearMissions();
            _missionsManager.EnsureMaxMission();
            _missionsManager.StartMissions();
        }
    }
}