using System;
using Missions.MissionsSource;
using ModelView;
using UnityEditor;
using UnityEngine;

namespace Missions.UI
{
    public class MissionsUI : MonoBehaviour
    {
        [SerializeField] private ViewList _viewList;
        [SerializeReference, SubclassSelector] private IMissionListSource _missionListSource;

        private void Start()
        {
            UpdateMissionsList();
            _missionListSource.ListChanged += UpdateMissionsList;
        }

        private void UpdateMissionsList()
        {
            _viewList.Clear();
            foreach (var mission in _missionListSource.GetMissions())
            {
                AddMissionView(mission);
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