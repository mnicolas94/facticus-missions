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
        public static string MilestonesModePropertyName => nameof(_milestonesMode);
        public static string IntervalPropertyName => nameof(_interval);
        public static string IntervalMilestonePropertyName => nameof(_intervalMilestone);
        public static string MilestonesPropertyName => nameof(_milestones);
        
        [SerializeField] public MissionData MissionAsset;
        
        public enum MilestonesMode { Interval, Manual }
        [SerializeField] private MilestonesMode _milestonesMode;
        public MilestonesMode MilestonesInputMode => _milestonesMode;

        [SerializeField] private float _interval;
        [SerializeField] private ProgressMilestone _intervalMilestone;
        
        [SerializeField] private List<ProgressMilestone> _milestones;

        private  List<ProgressMilestone> _intervalMilestones;
        
        public List<ProgressMilestone> Milestones
        {
            get
            {
                // manual
                if (_milestonesMode == MilestonesMode.Manual)
                {
                    return _milestones;
                }
                
                // interval
                if (_intervalMilestones == null)
                {
                    _intervalMilestones = new List<ProgressMilestone>();
                    var currentThreshold = _interval;
                    while (currentThreshold < 1f)
                    {
                        var milestone = _intervalMilestone.Clone();
                        milestone.ProgressThreshold = currentThreshold;
                        _intervalMilestones.Add(milestone);
                        currentThreshold += _interval;
                    }
                }

                return _intervalMilestones;
            }
        }
        
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

        public ProgressMilestone Clone()
        {
            return new ProgressMilestone()
            {
                ProgressThreshold = ProgressThreshold,
                Reached = Reached,
                IndicatorNotReachedPrefabOverride = IndicatorNotReachedPrefabOverride,
                IndicatorReachedPrefabOverride = IndicatorReachedPrefabOverride
            };
        }
    }
}