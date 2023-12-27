﻿using System;
using System.Collections.Generic;
using System.Linq;
using ModelView;
using UnityEngine;

namespace Missions.UI
{
    public class MissionsUI : MonoBehaviour
    {
        [SerializeField] private MissionsSerializableList _missionsList;
        [SerializeField] private ViewList _viewList;

        private void Start()
        {
            InitializeFirstMissions();
            _missionsList.Added += AddMissionView;
            _missionsList.Removed += RemoveMissionView;
        }

        private void InitializeFirstMissions()
        {
            _viewList.PopulateModels(_missionsList.Cast<object>().ToList());
        }

        private void AddMissionView(IMission mission)
        {
            _viewList.Add(mission);
        }

        private void RemoveMissionView(IMission mission)
        {
            _viewList.Remove(mission);
        }
    }
}