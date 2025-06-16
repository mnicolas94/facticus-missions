using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.Properties;
using UnityEngine;

namespace Missions
{
    [GeneratePropertyBag]
    [CreateAssetMenu(fileName = "MissionsSerializableState", menuName = "Facticus/Missions/Missions serializable state", order = 0)]
    public partial class MissionsSerializableState : ScriptableObject
    {
        [SerializeField] private string _lastRefreshTime;
        public string LastRefreshTime => _lastRefreshTime;

        [SerializeReference] private List<IMission> _missions = new();
        public ReadOnlyCollection<IMission> Missions => _missions.AsReadOnly();

        [DontCreateProperty] public Action<IMission> Added;
        /// <summary>
        /// Invoked when a mission is removed from the list. Won't be invoked when the list is cleared, use the
        /// Cleared event instead.
        /// </summary>
        [DontCreateProperty] public Action<IMission> Removed;
        [DontCreateProperty] public Action Cleared;

        public void UpdateRefreshTime()
        {
            _lastRefreshTime = DateTime.UtcNow.ToString("o");
        }
        
        public void Add(IMission item)
        {
            _missions.Add(item);
            Added?.Invoke(item);
        }

        public void Clear()
        {
            _missions.Clear();
            Cleared?.Invoke();
        }

        public bool Remove(IMission item)
        {
            var result = _missions.Remove(item);
            if (result)
            {
                Removed?.Invoke(item);
            }
            return result;
        }
    }
}