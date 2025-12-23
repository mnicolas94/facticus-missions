using System;
using System.Collections.Generic;
using SerializableCallback;
using UnityEngine;
using UnityEngine.Events;
using Utils.Extensions;

namespace Missions
{
    /// <summary>
    /// Controls the logic of how to create, start and end missions.
    /// </summary>
    public class MissionsManager : MonoBehaviour
    {
        [SerializeField] private MissionsSerializableState _currentMissions;
        public MissionsSerializableState CurrentMissions => _currentMissions;

        enum StartMissionsMode
        {
            Awake, Start, MethodCall
        }
        [SerializeField] private StartMissionsMode _startMissionsMode = StartMissionsMode.MethodCall;

        public Action<MissionData> OnBeforeMissionInitialized { get; set; }

        [SerializeField] private UnityEvent<MissionData> _onMissionCompleted;
        public UnityEvent<MissionData> OnMissionCompleted => _onMissionCompleted;
        
        private void Awake()
        {
            if (_startMissionsMode == StartMissionsMode.Awake)
            {
                StartMissions();
            }
        }
        
        private void Start()
        {
            if (_startMissionsMode == StartMissionsMode.Start)
            {
                StartMissions();
            }
        }

        public void ClearMissions()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
#endif
                foreach (var currentMission in _currentMissions.Missions)
                {
                    currentMission.Mission.EndMission();
                }
#if UNITY_EDITOR
            }
#endif

            _currentMissions.Clear();
        }

        public void CreateMissionsFromAssets(List<MissionData> missionAssets)
        {
            foreach (var missionAsset in missionAssets)
            {
                CreateNewMissionFromAsset(missionAsset);
            }
        }

        public MissionData CreateNewMissionFromAsset(MissionData missionOriginalAsset)
        {
            var mission = missionOriginalAsset.Clone();
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
#endif
                OnBeforeMissionInitialized?.Invoke(mission);
                mission.Initialize();
#if UNITY_EDITOR
            }
#endif
            _currentMissions.Add(missionOriginalAsset, mission);
            return mission;
        }

        public void StartMissions()
        {
            foreach (var serializableMission in _currentMissions.Missions)
            {
                StartMission(serializableMission.Mission);
            }
        }

        public void StartMission(MissionData mission)
        {
            if (mission.IsCompleted) return;  // do not start if already completed
            
            mission.StartMission();
            mission.OnCompleted += () => CompleteMission(mission);
        }

        private void CompleteMission(MissionData mission)
        {
            mission.EndMission();
            if (!mission.RequiresClaim)
            {
                mission.ApplyReward();
            }
            _onMissionCompleted.Invoke(mission);
        }
    }
}
