using System;
using System.Collections.Generic;
using UnityEngine;

namespace Missions.MissionsSource
{
    [Serializable]
    public class SerializedMissionsSource : IMissionListSource
    {
        [SerializeField] private List<MissionsSerializableState> _lists;

        private Action _listChanged;
        public Action ListChanged
        {
            get
            {
                foreach (var missionsSerializableState in _lists)
                {
                    missionsSerializableState.Added -= NotifyChange;
                    missionsSerializableState.Removed -= NotifyChange;
                    missionsSerializableState.Cleared -= NotifyChange;
                    
                    missionsSerializableState.Added += NotifyChange;
                    missionsSerializableState.Removed += NotifyChange;
                    missionsSerializableState.Cleared += NotifyChange;
                }
                return _listChanged;
            }
            set => _listChanged = value;
        }

        public SerializedMissionsSource()
        {
        }

        public SerializedMissionsSource(List<MissionsSerializableState> lists)
        {
            _lists = lists;
        }

        private void NotifyChange()
        {
            _listChanged?.Invoke();
        }

        private void NotifyChange(MissionData _)
        {
            NotifyChange();
        }

        public IEnumerable<MissionData> GetMissions()
        {
            foreach (var list in _lists)
            {
                foreach (var serializableMission in list.Missions)
                {
                    yield return serializableMission.Mission;
                }
            }
        }
    }
}