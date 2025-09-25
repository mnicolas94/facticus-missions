using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Utils.Serializables;

namespace Missions
{
    [CreateAssetMenu(fileName = "MissionsPoolData", menuName = "Facticus/Missions/MissionsPoolData", order = 0)]
    public class MissionsPoolData : ScriptableObject
    {
        [SerializeField] private List<MissionData> _missions = new();
        public List<MissionData> Missions => _missions;
        
        [SerializeField] private SerializableTimeSpan _refreshTimeSpan;
        public SerializableTimeSpan RefreshTimeSpan => _refreshTimeSpan;

        public static int SecondsToNextRefresh(MissionsPoolData missionsData, MissionsSerializableState persistedData)
        {
            var serializedTime = persistedData.LastRefreshTime;
            var isValidDate = DateTime.TryParse(serializedTime, null, DateTimeStyles.RoundtripKind, out var parsed);
            var lastRefresh = isValidDate ? parsed : DateTime.MinValue;
            return missionsData.RefreshTimeSpan.ToSeconds() - (int)(DateTime.UtcNow - lastRefresh).TotalSeconds;
        }
    }
}