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
        private float _lastSaveTime;
        private bool _isOnCooldown;
        
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
            mission.Reward.RewardClaimed += SaveNotOften;
            
            if (mission.Mission is IMissionProgress progress)
            {
                progress.OnProgressChanged += SaveNotOften;
            }
        }

        /// <summary>
        /// Saves the missions list. If the request to save is too often, the save will be performed after
        /// a cooldown. It ignores any save request during the cooldown.
        /// </summary>
        private void SaveNotOften()
        {
            var elapsed = Time.time - _lastSaveTime;
            
            if (elapsed < _saveCooldown)  // trying to save too early
            {
                if (!_isOnCooldown)
                {
                    SaveAfterCooldown(_saveCooldown);  // put save on cooldown
                }
                return;
            }

            SaveList();
        }

        private async void SaveAfterCooldown(float cooldown)
        {
            _isOnCooldown = true;
            
            await Awaitable.WaitForSecondsAsync(cooldown);
            SaveList();
            
            _isOnCooldown = false;
        }
        
        private void SaveList(MissionData _)
        {
            SaveList();
        }
        
        private async void SaveList()
        {
            _lastSaveTime = Time.time;
            await _missionsList.Save();
        }
    }
}
#endif