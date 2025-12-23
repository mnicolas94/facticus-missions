using System;
using UnityEngine;
using Utils.Attributes;

namespace Missions.CreationStrategies
{
    /// <summary>
    /// Will create all missions at once from the pool. Useful for achievements.
    /// </summary>
    public class MissionCreatorAllOnce : MonoBehaviour
    {
        [SerializeField, AutoProperty] private MissionsManager _missionsManager;
        [SerializeField] private MissionsPoolData _missionsPool;

        private void Start()
        {
            // create missions
            foreach (var missionAsset in _missionsPool.Missions)
            {
                if (!_missionsManager.CurrentMissions.TryGetMissionFromOriginal(missionAsset, out _))
                {
                    var missionInstance = _missionsManager.CreateNewMissionFromAsset(missionAsset);
                    _missionsManager.StartMission(missionInstance);
                }
            }
        }
    }
}