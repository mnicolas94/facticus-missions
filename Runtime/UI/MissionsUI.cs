using System;
using ModelView;
using UnityEngine;

namespace Missions.UI
{
    public class MissionsUI : MonoBehaviour
    {
        [SerializeField] private MissionsSerializableState _missionsList;
        [SerializeField] private ViewList _viewList;

        private void Start()
        {
            InitializeFirstMissions();
            _missionsList.Added += AddMissionView;
            _missionsList.Removed += RemoveMissionView;
            _missionsList.Cleared += ClearMissions;
        }

        private void InitializeFirstMissions()
        {
            foreach (var serializableMission in _missionsList.Missions)
            {
                AddMissionView(serializableMission.Mission);
            }
        }

        private void AddMissionView(MissionData mission)
        {
            _viewList.Add(mission);
        }

        private void RemoveMissionView(MissionData mission)
        {
            _viewList.Remove(mission);
        }

        private void ClearMissions()
        {
            _viewList.Clear();
        }
    }
}