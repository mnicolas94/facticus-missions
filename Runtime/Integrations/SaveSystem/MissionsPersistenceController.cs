#if ENABLED_SAVESYSTEM
using System.Collections.Generic;
using UnityEngine;
using SaveSystem;
using Utils.Attributes;

namespace Missions.Integrations.SaveSystem
{
    public class MissionsPersistenceController : MonoBehaviour
    {
        [SerializeField, AutoProperty] private MissionsManager _missionsManager;
        private MissionsSerializableState _missionsList;
        
        private void Start()
        {
            _missionsList = _missionsManager.CurrentMissions;
            _missionsList.Added += OnMissionAdded;
            _missionsList.Removed += SaveList;
            _missionsList.Cleared += SaveList;

            foreach (var mission in _missionsList.Missions)
            {
                RegisterOnProgressChanged(mission.Mission);
            }
        }

        private void OnDestroy()
        {
            UnregisterListeners();
        }
        
        public void UnregisterListeners()
        {
            _missionsList.Added -= OnMissionAdded;
            _missionsList.Removed -= SaveList;
            _missionsList.Cleared -= SaveList;
        }

        private void OnMissionAdded(MissionData mission)
        {
            RegisterOnProgressChanged(mission);
            SaveList();
        }

        private void RegisterOnProgressChanged(MissionData mission)
        {
            if (mission.Mission is IMissionProgress progress)
            {
                progress.OnProgressChanged += SaveListOnProgressChanged;
            }
        }

        private void SaveListOnProgressChanged()
        {
            // TODO add a cooldown to avoid to many saves for missions that progress to often
            SaveList();
        }

        private void SaveList(MissionData _)
        {
            SaveList();
        }
        
        private async void SaveList()
        {
            await _missionsList.Save();
        }
    }
}
#endif