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
        public ReadOnlyCollection<SerializableMission> Missions => _missions.AsReadOnly();

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
        
        public void Add(MissionData missionAsset, MissionData mission)
        {
            _missions.Add(new SerializableMission(missionAsset, mission));
            Added?.Invoke(mission);
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

        public bool TryGetMissionFromOriginal(MissionData originalAsset, out MissionData mission)
        {
            foreach (var serializableMission in _missions)
            {
                if (serializableMission.OriginalMissionAsset == originalAsset)
                {
                    mission = serializableMission.Mission;
                    return true;
                }
            }

            mission = null;
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