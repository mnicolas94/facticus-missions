using System;
using Unity.Properties;
using UnityEngine;

namespace Missions
{
    [Serializable]
    [GeneratePropertyBag]
    public partial class SerializableMission
    {
        [SerializeField] private MissionData _originalMissionAsset;
        public MissionData OriginalMissionAsset => _originalMissionAsset;
        
        [SerializeField] private MissionDataSerializable _serialized;
        
        private MissionData _mission;
        public MissionData Mission
        {
            get
            {
                if (!_mission)
                {
                    _mission = _originalMissionAsset.Clone();  // to get non-persistent fields from original asset
                    _mission.SerializableData = _serialized;
                }
    
                return _mission;
            }
        }
    
        public SerializableMission()
        {
        }
        
        public SerializableMission(MissionData originalAsset, MissionData mission)
        {
            _originalMissionAsset = originalAsset;
            _mission = mission;
            _serialized = mission.SerializableData;
        }
    }
}