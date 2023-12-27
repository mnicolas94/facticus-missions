using System;
using UnityEngine;
using UnityEngine.Localization;

namespace Missions
{
    [Serializable]
    public abstract class BaseMission : IMission
    {
        [SerializeReference, SubclassSelector] private IMissionReward _reward;
        [SerializeField] private LocalizedString _description;

        public IMission Clone()
        {
            var json = JsonUtility.ToJson(this);
            var type = GetType();
            return JsonUtility.FromJson(json, type) as IMission;
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

        public abstract void StartMission(Action onCompleted);

        public abstract void EndMission();
    }
}