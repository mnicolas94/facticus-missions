using System;
using System.Collections.Generic;
using ModelView;
using UnityEngine;
using UnityEngine.Pool;

namespace Missions.UI
{
    public class MissionsUI : MonoBehaviour
    {
        [SerializeField] private ViewList _viewList;
        [SerializeReference, SubclassSelector] private IMissionListSource _missionListSource;

        private readonly List<MissionEventsHandler> _missionEventsHandlers = new();
        
        private void Start()
        {
            UpdateMissionsList();
            _missionListSource.ListChanged += UpdateMissionsList;
        }

        private void OnDestroy()
        {
            _missionListSource.ListChanged -= UpdateMissionsList;
            ReleaseHandlers();
        }

        private void UpdateMissionsList()
        {
            _viewList.Clear();
            ReleaseHandlers();
            
            // get missions list
            using var _ = ListPool<MissionData>.Get(out var missions);
            var missionsEnumerable = _missionListSource.GetMissions();
            missions.AddRange(missionsEnumerable);
            
            // sort by claimable first
            missions.Sort((a, b) =>
            {
                var aPriority = a.RequiresClaim && a.IsCompleted && !a.IsRewardClaimed ? 0 : 1;
                var bPriority = b.RequiresClaim && b.IsCompleted && !b.IsRewardClaimed ? 0 : 1;
                return aPriority.CompareTo(bPriority);
            });
            
            foreach (var mission in missions)
            {
                AddMissionView(mission);
                _missionEventsHandlers.Add(new MissionEventsHandler(mission, SortList));
            }
        }

        private void SortList()
        {
            using var _ = ListPool<MissionData>.Get(out var missions);
            var missionsEnumerable = _missionListSource.GetMissions();
            missions.AddRange(missionsEnumerable);
            
            var views = _viewList.GetViews<MissionView>();
            var index = 0;
            foreach (var missionView in views)
            {
                var mission = missionView.Model;
                var siblingIndex = 0;
                if (mission.IsCompleted && !mission.IsRewardClaimed)
                {
                    siblingIndex = index;
                    index++;
                }
                else
                {
                    siblingIndex = missions.IndexOf(mission) + index;
                }
                missionView.transform.SetSiblingIndex(siblingIndex);
            }
        }

        private void AddMissionView(MissionData mission)
        {
            _viewList.Add(mission);
        }

        private void RemoveMissionView(MissionData mission)
        {
            _viewList.Remove(mission);
        }

        private void ClearMissions()
        {
            _viewList.Clear();
        }

        private void ReleaseHandlers()
        {
            foreach (var missionEventsHandler in _missionEventsHandlers)
            {
                missionEventsHandler.Release();
            }
            _missionEventsHandlers.Clear();
        }
    }

    internal struct MissionEventsHandler
    {
        public MissionData Mission;
        public Action SortListCallback;

        public MissionEventsHandler(MissionData mission, Action sortListCallback)
        {
            Mission = mission;
            SortListCallback = sortListCallback;
            
            if (mission.IsCompleted && !mission.IsRewardClaimed)
            {
                mission.Reward.RewardClaimed += OnRewardClaimed;
            }
            else if (!mission.IsCompleted)
            {
                mission.OnCompleted += OnCompleted;
            }
            
        }

        private void OnCompleted()
        {
            SortListCallback?.Invoke();
            Mission.OnCompleted -= OnCompleted;
            Mission.Reward.RewardClaimed += OnRewardClaimed;
        }

        private void OnRewardClaimed()
        {
            SortListCallback?.Invoke();
            Release();
        }

        public void Release()
        {
            Mission.Reward.RewardClaimed -= OnRewardClaimed;
            Mission.OnCompleted -= OnCompleted;
        }
    }
}