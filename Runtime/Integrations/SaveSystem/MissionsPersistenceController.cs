#if ENABLED_SAVESYSTEM
using UnityEngine;
using SaveSystem;
using Utils.Attributes;

namespace Missions.Integrations.SaveSystem
{
    public class MissionsPersistenceController : MonoBehaviour
    {
        [SerializeField, AutoProperty] private MissionsManager _missionsManager;
        
        [Tooltip("Cooldown in seconds to avoid saving too often due to missions that progress to rapidly")]
        [SerializeField] private float _saveCooldown = 1f;
        
        private MissionsSerializableState _missionsList;
        
        private void Start()
        {
            _missionsList = _missionsManager.CurrentMissions;
            _missionsList.Added += OnMissionAdded;
            _missionsList.Removed += OnMissionRemoved;
            _missionsList.Cleared += SaveList;

            foreach (var mission in _missionsList.Missions)
            {
                RegisterMissionListeners(mission.Mission);
            }
        }

        private void OnDestroy()
        {
            UnregisterListeners();
        }
        
        public void UnregisterListeners()
        {
            if (!_missionsList) return;
            _missionsList.Added -= OnMissionAdded;
            _missionsList.Removed -= OnMissionRemoved;
            _missionsList.Cleared -= SaveList;
        }

        private void OnMissionAdded(MissionData mission)
        {
            RegisterMissionListeners(mission);
            SaveList();
        }

        private void OnMissionRemoved(MissionData mission)
        {
            SaveList();
        }

        private void RegisterMissionListeners(MissionData mission)
        {
            mission.Reward.RewardClaimed += SaveList;
            
            if (mission.Mission is IMissionProgress progress)
            {
                progress.OnProgressChanged += SaveList;
            }
        }
        
        private void SaveList(MissionData _)
        {
            SaveList();
        }
        
        private async void SaveList()
        {
            await _missionsList.SaveNotOften(_saveCooldown);
        }
    }
}
#endif