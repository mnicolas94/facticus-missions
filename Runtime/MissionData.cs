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
        
        [SerializeReference, SubclassSelector] protected IMissionReward _reward;
        
        [SerializeReference, SubclassSelector] private IMissionImplementation _mission;
        public IMissionImplementation Mission => _mission;
        
        public LocalizedString GetDescription()
        {
            return _description;
        }
        
        public IMissionReward GetReward()
        {
            return _reward;
        }

        public MissionData Clone()
        {
            return Instantiate(this);
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

        public Action OnCompleted
        {
            get => _mission.OnCompleted;
            set => _mission.OnCompleted = value;
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