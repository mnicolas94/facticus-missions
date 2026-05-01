using System;
using System.Collections.Generic;
using Missions.UI;
using ModelView;
using UnityEngine;
using UnityEngine.Events;

namespace Missions
{
    public class MissionProgressMilestonesObserver : MonoBehaviour
    {
        [SerializeField] private MissionsManager _missionsManager;
        [SerializeField] private MissionsUI _missionsUI;
        [SerializeField] private List<MissionMilestones> _missionsMilestones;

        private void Start()
        {
            _missionsManager.MissionStarted.AddListener(OnMissionStarted);
            _missionsUI.MissionViewAdded += OnMissionUiAdded;
        }

        private void OnDestroy()
        {
            _missionsManager.MissionStarted.RemoveListener(OnMissionStarted);
            _missionsUI.MissionViewAdded -= OnMissionUiAdded;
            
            foreach (var missionsMilestone in _missionsMilestones)
            {
                missionsMilestone.Dispose();
            }
        }

        private void OnMissionStarted(MissionData missionInstance)
        {
            if (missionInstance.Mission is not IMissionProgress) return;
            
            if (TryGetMilestonesForMission(missionInstance, out var missionMilestones))
            {
                missionMilestones.Initialize(missionInstance);
            }
        }

        private void OnMissionUiAdded(IView view)
        {
            if (view is not MissionView missionView) return;
            
            var missionData = missionView.Model;
            if (!TryGetMilestonesForMission(missionData, out var missionMilestones)) return;
            
            if (!missionView.TryGetComponent<ProgressMilestonesView>(out var milestonesView)) return;
            
            milestonesView.SpawnMilestoneIndicators(missionMilestones);
        }

        private bool TryGetMilestonesForMission(MissionData missionInstance, out MissionMilestones milestones)
        {
            foreach (var missionMilestone in _missionsMilestones)
            {
                var missionsStorage = _missionsManager.CurrentMissions;
                // check if mission asset has an instance in the missions storage
                if (!missionsStorage.TryGetMissionFromOriginal(missionMilestone.MissionAsset, out var instance)) continue;
                
                if (instance == missionInstance)
                {
                    milestones = missionMilestone;
                    return true;
                }
            }

            milestones = null;
            return false;
        }
    }

    [Serializable]
    public class MissionMilestones : IDisposable
    {
        [SerializeField] public MissionData MissionAsset;
        [SerializeField] public List<ProgressMilestone> Milestones;

        public MissionData MissionInstance { get; set; }
        public IMissionProgress MissionProgress { get; set; }
        public float LastTrackedProgress { get; set; }

        public void Initialize(MissionData missionInstance)
        {
            if (missionInstance.Mission is not IMissionProgress missionProgress) return;
            
            MissionInstance = missionInstance;
            MissionProgress = missionProgress;
            LastTrackedProgress = missionProgress.GetCurrentProgress();
            
            missionProgress.OnProgressChanged += OnProgressChanged;
        }

        public void Dispose()
        {
            if (MissionProgress != null)
            {
                MissionProgress.OnProgressChanged -= OnProgressChanged;
            }
        }

        private void OnProgressChanged()
        {
            var maxProgress = MissionProgress.GetMaxProgress();
            var lastNormalized = LastTrackedProgress / maxProgress;
            var currentProgress = MissionProgress.GetCurrentProgress();
            var currentNormalized = currentProgress / maxProgress;
            foreach (var milestone in Milestones)
            {
                if (lastNormalized < milestone.ProgressThreshold && currentNormalized >= milestone.ProgressThreshold)
                {
                    milestone.Reached.Invoke();
                }
            }

            LastTrackedProgress = currentProgress;
        }
    }
    
    [Serializable]
    public class ProgressMilestone
    {
        [SerializeField] public float ProgressThreshold;
        [SerializeField] public UnityEvent Reached;
        
        [Header("UI")]
        [SerializeField] public RectTransform IndicatorNotReachedPrefabOverride;
        [SerializeField] public RectTransform IndicatorReachedPrefabOverride;
    }
}