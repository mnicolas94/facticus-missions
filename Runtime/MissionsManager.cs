using SerializableCallback;
using UnityEngine;
using UnityEngine.Events;
using Utils.Extensions;

namespace Missions
{
    public class MissionsManager : MonoBehaviour
    {
        [SerializeField] private MissionsSerializableState _currentMissions;
        public MissionsSerializableState CurrentMissions => _currentMissions;

        [SerializeField] private MissionsPoolData _missionsPool;
        public MissionsPoolData MissionsPool => _missionsPool;

        [SerializeField] private SerializableValueCallback<int> _maxMissions;
        [SerializeField] private SerializableValueCallback<bool> _canCreateMission;
        [SerializeField] private bool _createNewOnCompleted;

        enum StartMissionsMode
        {
            Awake, Start, MethodCall
        }
        [SerializeField] private StartMissionsMode _startMissionsMode = StartMissionsMode.MethodCall;

        [SerializeField] private UnityEvent _onMissionCompleted;
        public UnityEvent OnMissionCompleted => _onMissionCompleted;
        
        [SerializeField] private UnityEvent<MissionData> _onMissionCompletedWithArg;
        public UnityEvent<MissionData> OnMissionCompletedWithArg => _onMissionCompletedWithArg;

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
            foreach (var currentMission in _currentMissions.Missions)
            {
                currentMission.Mission.EndMission();
            }

            _currentMissions.Clear();
        }

        /// <summary>
        /// Create new missions up to _maxMissions value
        /// </summary>
        public void EnsureMaxMission()
        {
            var missing = _maxMissions.Value - _currentMissions.Missions.Count;
            CreateNewMissions(missing);
        }

        public void StartMissions()
        {
            foreach (var mission in _currentMissions.Missions)
            {
                StartMission(mission);
            }
        }

        private void CreateNewMissions(int count)
        {
            for (int i = 0; i < count; i++)
            {
                TryCreateNewMission();
            }
        }

        private bool TryCreateNewMission()
        {
            return TryCreateNewMission(out var _);
        }

        private bool TryCreateNewMission(out MissionData mission)
        {
            if (_canCreateMission.Value)
            {
                mission = _missionsPool.Missions.GetRandom().Clone();
                _currentMissions.Add(mission);
                mission.Initialize();
                return true;
            }

            mission = default;
            return false;
        }

        private void StartMission(MissionData mission)
        {
            mission.StartMission();
            mission.OnCompleted += () => CompleteMission(mission);
        }

        private void CompleteMission(MissionData mission)
        {
            mission.EndMission();
            mission.GetReward().ApplyReward();
            _onMissionCompleted.Invoke();
            _onMissionCompletedWithArg.Invoke(mission);
            
            _currentMissions.Remove(mission);
            if (_createNewOnCompleted && TryCreateNewMission(out var newMission))
            {
                StartMission(newMission);
            }
        }
    }
}
