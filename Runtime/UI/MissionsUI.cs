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
        [SerializeField] private MissionsSerializableState _missionsList;
        [SerializeReference, SubclassSelector] private IMissionListSource _missionListSource;

        private void Start()
        {
            UpdateMissionsList();
            _missionListSource.ListChanged += UpdateMissionsList;
            // _missionsList.Added += AddMissionView;
            // _missionsList.Removed += RemoveMissionView;
            // _missionsList.Cleared += ClearMissions;
        }

        private void UpdateMissionsList()
        {
            _viewList.Clear();
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

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_missionListSource == null)
            {
                _missionListSource = new SerializedMissionsSource(new() { _missionsList });
                EditorUtility.SetDirty(this);
            }
        }
#endif
    }
}