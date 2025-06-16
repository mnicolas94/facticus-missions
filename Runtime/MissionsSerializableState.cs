using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.Properties;
using UnityEngine;

namespace Missions
{
    [GeneratePropertyBag]
    [CreateAssetMenu(fileName = "MissionsSerializableState", menuName = "Facticus/Missions/Missions serializable state", order = 0)]
    public partial class MissionsSerializableState : ScriptableObject
    {
        [SerializeField] private string _lastRefreshTime;
        public string LastRefreshTime => _lastRefreshTime;

        [SerializeField] private List<SerializableMission> _missions = new();
        public List<SerializableMission> Missions => _missions;

        [DontCreateProperty] public Action<MissionData> Added;
        /// <summary>
        /// Invoked when a mission is removed from the list. Won't be invoked when the list is cleared, use the
        /// Cleared event instead.
        /// </summary>
        [DontCreateProperty] public Action<MissionData> Removed;
        [DontCreateProperty] public Action Cleared;

        public void UpdateRefreshTime()
        {
            _lastRefreshTime = DateTime.UtcNow.ToString("o");
        }
        
        public void Add(MissionData item)
        {
            _missions.Add(new SerializableMission(item));
            Added?.Invoke(item);
        }

        public void Clear()
        {
            _missions.Clear();
            Cleared?.Invoke();
        }

        public bool Remove(MissionData item)
        {
            if (TryGetSerializableMissionFromMission(item, out var serializableMission))
            {
                _missions.Remove(serializableMission);
                Removed?.Invoke(item);
                return true;
            }
            return false;
        }
        
        private bool TryGetSerializableMissionFromMission(MissionData mission, out SerializableMission serializableMission)
        {
            foreach (var serializable in _missions)
            {
                if (serializable.Mission == mission)
                {
                    serializableMission = serializable;
                    return true;
                }
            }
            serializableMission = default;
            return false;
        }
    }
}