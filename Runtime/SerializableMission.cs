using System;
using Unity.Properties;
using Unity.Serialization.Json;
using UnityEngine;

namespace Missions
{
    [Serializable]
    [GeneratePropertyBag]
    public partial class SerializableMission : ISerializationCallbackReceiver
    {
        private static readonly JsonSerializationParameters SerializationParameters = new ()
        {
            DisableRootAdapters = true,
            Minified = true,
            Simplified = true,
            DisableValidation = true,
        };

        [SerializeField] private MissionData _originalMissionAsset;
        [SerializeField] private string _serializedData;
        
        private MissionData _mission;

        public MissionData Mission
        {
            get
            {
                if (!_mission && !string.IsNullOrEmpty(_serializedData))
                {
                    _mission = _originalMissionAsset.Clone();  // to get non-persistent fields from original asset
                    JsonSerialization.FromJsonOverride(_serializedData, ref _mission, SerializationParameters);
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
        }
        
        public void OnBeforeSerialize()
        {
            if (!_mission) return;
            _serializedData = JsonSerialization.ToJson(_mission, SerializationParameters);
        }

        public void OnAfterDeserialize()
        {
        }
    }
}