using System;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using Utils.Attributes;

namespace Missions
{
    [GeneratePropertyBag]
    [CreateAssetMenu(fileName = "Mission", menuName = "Facticus/Missions/Mission", order = 0)]
    public partial class MissionData : ScriptableObject, IMissionImplementation
    {
        private const string DescriptionSelfKey = "self";
        
        [SerializeField, DontCreateProperty] private LocalizedString _description;
        public LocalizedString Description => _description;

        [SerializeField, DontCreateProperty] private bool _requiresClaim;
        public bool RequiresClaim => _requiresClaim;
        
        [DontCreateProperty]
        public Action RewardClaimed;
        
        [SerializeField] private MissionDataSerializable _serializableData;
        public MissionDataSerializable SerializableData => _serializableData;

        public IMissionReward Reward => _serializableData.Reward;

        public IMissionImplementation Mission => _serializableData.Mission;

        public bool IsRewardClaimed => _serializableData.IsClaimed;

        public MissionData Clone()
        {
            return Instantiate(this);
        }

        public bool IsCompleted => Mission.IsCompleted;

        public Action OnCompleted
        {
            get => Mission.OnCompleted;
            set => Mission.OnCompleted = value;
        }
        
        public void Initialize()
        {
            Mission.Initialize();
            Reward.Initialize();
        }

        public void StartMission()
        {
            Mission.StartMission();
        }

        public void EndMission()
        {
            Mission.EndMission();
        }

        public void ApplyReward()
        {
            if (IsRewardClaimed) return;
            Reward.ApplyReward();
            _serializableData.IsClaimed = true;
            RewardClaimed?.Invoke();
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

    /// <summary>
    /// Serializable version of the mission data. This is the data that is persisted (saved).
    /// </summary>
    [Serializable]
    [GeneratePropertyBag]
    public class MissionDataSerializable
    {
        [SerializeField, ReadOnly] public bool IsClaimed;
        
        [SerializeReference, SubclassSelector] public IMissionReward Reward;

        [SerializeReference, SubclassSelector] public IMissionImplementation Mission;
    }
}