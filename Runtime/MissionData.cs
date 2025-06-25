using System;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace Missions
{
    [GeneratePropertyBag]
    [CreateAssetMenu(fileName = "Mission", menuName = "Facticus/Missions/Mission", order = 0)]
    public partial class MissionData : ScriptableObject, IMissionImplementation
    {
        private const string DescriptionSelfKey = "self";
        
        [SerializeField, DontCreateProperty] private LocalizedString _description;
        public LocalizedString Description => _description;

        [SerializeReference, SubclassSelector] protected IMissionReward _reward;
        public IMissionReward Reward
        {
            get => _reward;
            set => _reward = value;
        }

        [SerializeReference, SubclassSelector] private IMissionImplementation _mission;
        public IMissionImplementation Mission
        {
            get => _mission;
            set => _mission = value;
        }

        public MissionData Clone()
        {
            return Instantiate(this);
        }

        public bool IsCompleted => _mission.IsCompleted;

        public Action OnCompleted
        {
            get => _mission.OnCompleted;
            set => _mission.OnCompleted = value;
        }
        
        public void Initialize()
        {
            _mission.Initialize();
            _reward.Initialize();
        }

        public void StartMission()
        {
            _mission.StartMission();
        }

        public void EndMission()
        {
            _mission.EndMission();
        }

#if UNITY_EDITOR
        
        private void OnValidate()
        {
            if (!_description.ContainsKey(DescriptionSelfKey))
            {
                _description.Add(DescriptionSelfKey, new ObjectVariable(){ Value = this});
                EditorUtility.SetDirty(this);
            }
        }
        
#endif
    }
}