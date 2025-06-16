using System;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Localization;

namespace Missions
{
    [Serializable]
    [GeneratePropertyBag]
    public abstract partial class BaseMission : IMission
    {
        [SerializeReference, SubclassSelector] protected IMissionReward _reward;
        [SerializeField] protected LocalizedString _description;

        public IMission Clone()
        {
            var json = JsonUtility.ToJson(this);
            var type = GetType();
            var clon = JsonUtility.FromJson(json, type) as IMission;
            return clon;
        }

        public LocalizedString GetDescription()
        {
            return _description;
        }

        public IMissionReward GetReward()
        {
            return _reward;
        }

        public abstract void Initialize();

        public abstract void StartMission();

        public abstract void EndMission();
        
        public Action OnCompleted { get; set; }
    }
}