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
        [SerializeReference] private IMissionReward _reward;
        [SerializeReference] private IMissionImplementation _implementation;
        
        private MissionData _mission;
    
        public MissionData Mission
        {
            get
            {
                if (!_mission)
                {
                    _mission = _originalMissionAsset.Clone();  // to get non-persistent fields from original asset
                    _mission.Reward = _reward;
                    _mission.Mission = _implementation;
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
            _reward = mission.Reward;
            _implementation = mission.Mission;
        }
    }
}