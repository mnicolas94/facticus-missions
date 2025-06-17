#if ENABLED_SAVESYSTEM
using System.Collections.Generic;
using UnityEngine;
using SaveSystem;

namespace Missions.Integrations.SaveSystem
{
    public class MissionsPersistenceController : MonoBehaviour
    {
        [SerializeField] private List<MissionsSerializableState> _missionsLists;

        private readonly List<MissionSaver> _savers = new();
        
        private void Start()
        {
            foreach (var missionsList in _missionsLists)
            {
                _savers.Add(new MissionSaver(missionsList));
            }
        }

        private void OnDestroy()
        {
            foreach (var saver in _savers)
            {
                saver.UnregisterListeners();
            }
        }
    }

    public struct MissionSaver
    {
        private readonly MissionsSerializableState _missionsList;

        public MissionSaver(MissionsSerializableState missionsList)
        {
            _missionsList = missionsList;
            _missionsList.Added += OnMissionAdded;
            _missionsList.Removed += SaveList;
            _missionsList.Cleared += SaveList;

            foreach (var mission in missionsList.Missions)
            {
                RegisterOnProgressChanged(mission.Mission);
            }
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