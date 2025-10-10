using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Utils.Attributes;
using Utils.Extensions;

namespace Missions.CreationStrategies
{
    /// <summary>
    /// Will create and refresh missions in a cyclic way (e.g. daily, weekly)
    /// </summary>
    public class MissionsCyclicCreator : MonoBehaviour
    {
        [SerializeField, AutoProperty] private MissionsManager _missionsManager;
        [SerializeField] private MissionsPoolData _missionsPool;
        [SerializeField] private float _firstCheckDelay;
        [SerializeField] private int _maxMissions;

        private async void Start()
        {
            await Awaitable.WaitForSecondsAsync(_firstCheckDelay, destroyCancellationToken);
            CheckTimeInterval();
        }

        private async void CheckTimeInterval()
        {
            var secondsToNextRefresh = MissionsPoolData.SecondsToNextRefresh(_missionsPool,
                _missionsManager.CurrentMissions);

            await Awaitable.WaitForSecondsAsync(secondsToNextRefresh, destroyCancellationToken);

            if (destroyCancellationToken.IsCancellationRequested) return;
            
            Refresh();
            CheckTimeInterval();
        }

        public void Refresh()
        {
            _missionsManager.CurrentMissions.UpdateRefreshTime();
            _missionsManager.ClearMissions();
            using var _ = ListPool<MissionData>.Get(out var pool);
            GetRandomMissionsNonAlloc(pool);
            _missionsManager.CreateMissionsFromAssets(pool);
            _missionsManager.StartMissions();
        }

        private void GetRandomMissionsNonAlloc(List<MissionData> randomMissions)
        {
            var missionsPool = _missionsPool.Missions;
            randomMissions.AddRange(missionsPool);
            while (randomMissions.Count > _maxMissions)
            {
                randomMissions.PopRandom();
            }
            
            // if missions pool is smaller than max missions, we have to repeat some missions
            var repeated = Mathf.Max(0, _maxMissions - missionsPool.Count);
            for (int i = 0; i < repeated; i++)
            {
                var randomMissionRepeated = missionsPool.GetRandom();
                randomMissions.Add(randomMissionRepeated);
            }
        }
    }
}