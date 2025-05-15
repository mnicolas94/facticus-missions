#if ENABLED_SAVESYSTEM
using System.Collections.Generic;
using UnityEngine;
using SaveSystem;

namespace Missions.Integrations.SaveSystem
{
    public class MissionsPersistenceController : MonoBehaviour
    {
        [SerializeField] private List<MissionsSerializableList> _missionsLists;

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
        private readonly MissionsSerializableList _missionsList;

        public MissionSaver(MissionsSerializableList missionsList)
        {
            _missionsList = missionsList;
            _missionsList.Added += SaveList;
            _missionsList.Removed += SaveList;
            _missionsList.Cleared += SaveList;
        }

        public void UnregisterListeners()
        {
            _missionsList.Added -= SaveList;
            _missionsList.Removed -= SaveList;
        }

        private void SaveList(IMission _)
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