using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Missions.UI
{
    /// <summary>
    /// Handles milestones indicators over a Mission View's progress bar.
    /// </summary>
    public class ProgressMilestonesView : MonoBehaviour
    {
        [SerializeField] private Transform _progressBarContainer;
        [SerializeField] private RectTransform _milestoneIndicatorNotCompletedPrefab;
        [SerializeField] private RectTransform _milestoneIndicatorCompletedPrefab;

        private readonly List<ProgressMilestoneIndicator> _spawnedIndicators = new();

        private void OnDestroy()
        {
            ClearIndicators();
        }

        public void SpawnMilestoneIndicators(MissionMilestones milestones)
        {
            ClearIndicators();
            
            // spawn milestone indicators
            foreach (var milestone in milestones.Milestones)
            {
                // spawn indicator
                var isCompleted = milestone.ProgressThreshold <= milestones.MissionProgress.GetNormalizedProgress();
                var completedIndicatorPrefab = milestone.IndicatorReachedPrefabOverride
                    ? milestone.IndicatorReachedPrefabOverride
                    : _milestoneIndicatorCompletedPrefab;
                var notCompletedIndicatorPrefab = milestone.IndicatorNotReachedPrefabOverride 
                    ? milestone.IndicatorNotReachedPrefabOverride : _milestoneIndicatorNotCompletedPrefab;

                // store on list
                var indicatorData = new ProgressMilestoneIndicator(milestone, _progressBarContainer,
                    notCompletedIndicatorPrefab, completedIndicatorPrefab, isCompleted);
                _spawnedIndicators.Add(indicatorData);
            }
        }

        private void ClearIndicators()
        {
            foreach (var indicator in _spawnedIndicators)
            {
                indicator.Dispose();
            }
            _spawnedIndicators.Clear();
        }
    }
    
    public class ProgressMilestoneIndicator
    {
        private readonly Transform _indicatorsContainer;
        private readonly RectTransform _completedPrefab;
        private readonly ProgressMilestone _milestone;

        private RectTransform _indicatorInstance;

        public ProgressMilestoneIndicator(ProgressMilestone milestone, Transform indicatorsContainer, RectTransform notCompletedPrefab,
            RectTransform completedPrefab, bool isCompleted)
        {
            _indicatorsContainer = indicatorsContainer;
            _completedPrefab = completedPrefab;
            _milestone = milestone;
            
            var indicatorPrefab = isCompleted ? completedPrefab : notCompletedPrefab;
            _indicatorInstance = SpawnMilestoneIndicator(indicatorPrefab);

            if (!isCompleted)
            {
                milestone.Reached.AddListener(OnReached);
            }
        }

        private RectTransform SpawnMilestoneIndicator(RectTransform indicatorPrefab)
        {
            var normalizedProgress = _milestone.ProgressThreshold;
            var milestoneIndicator = Object.Instantiate(indicatorPrefab, _indicatorsContainer);
            var anchorMin = milestoneIndicator.anchorMin;
            anchorMin.x = normalizedProgress;
            milestoneIndicator.anchorMin = anchorMin;

            var anchorMax = milestoneIndicator.anchorMax;
            anchorMax.x = normalizedProgress;
            milestoneIndicator.anchorMax = anchorMax;
            return milestoneIndicator;
        }

        private void OnReached()
        {
            Object.Destroy(_indicatorInstance.gameObject);
            _indicatorInstance = SpawnMilestoneIndicator(_completedPrefab);
        }

        public void Dispose()
        {
            Object.Destroy(_indicatorInstance.gameObject);
            _milestone.Reached.RemoveListener(OnReached);
        }
    }
}