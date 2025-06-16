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
        
        [SerializeField] private string _serializedData;
        
        private MissionData _mission;

        public MissionData Mission
        {
            get
            {
                if (!_mission && !string.IsNullOrEmpty(_serializedData))
                {
                    _mission = JsonSerialization.FromJson<MissionData>(_serializedData, SerializationParameters);
                }

                return _mission;
            }
        }

        public SerializableMission()
        {
        }
        
        public SerializableMission(MissionData mission)
        {
            _mission = mission;
        }

        public static implicit operator MissionData(SerializableMission serializableMission)
        {
            return serializableMission.Mission;
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