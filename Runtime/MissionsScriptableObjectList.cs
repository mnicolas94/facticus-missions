using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Missions
{
    [CreateAssetMenu(fileName = "MissionsScriptableObjectList", menuName = "Facticus/Missions/Missions SO list", order = 0)]
    public class MissionsScriptableObjectList : ScriptableObject, IList<MissionScriptableObject> 
    {
        [SerializeField] private List<MissionScriptableObject> _missions;

        public Action<MissionScriptableObject> Added;
        public Action<MissionScriptableObject> Removed;
        
        public IEnumerator<MissionScriptableObject> GetEnumerator()
        {
            _missions ??= new List<MissionScriptableObject>();
            return _missions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_missions).GetEnumerator();
        }

        public void Add(MissionScriptableObject item)
        {
            _missions.Add(item);
            Added?.Invoke(item);
        }

        public void Clear()
        {
            var temp = new List<MissionScriptableObject>(_missions);
            _missions.Clear();
            foreach (var mission in temp)
            {
                Removed?.Invoke(mission);
            }
        }

        public bool Contains(MissionScriptableObject item)
        {
            return _missions.Contains(item);
        }

        public void CopyTo(MissionScriptableObject[] array, int arrayIndex)
        {
            _missions.CopyTo(array, arrayIndex);
        }

        public bool Remove(MissionScriptableObject item)
        {
            var result = _missions.Remove(item);
            if (result)
            {
                Removed?.Invoke(item);
            }
            return result;
        }

        public int Count => _missions.Count;

        public bool IsReadOnly => ((ICollection<MissionScriptableObject>)_missions).IsReadOnly;

        public int IndexOf(MissionScriptableObject item)
        {
            return _missions.IndexOf(item);
        }

        public void Insert(int index, MissionScriptableObject item)
        {
            _missions.Insert(index, item);
            Added?.Invoke(item);
        }

        public void RemoveAt(int index)
        {
            var item = _missions[index];
            _missions.RemoveAt(index);
            Removed?.Invoke(item);
        }

        public MissionScriptableObject this[int index]
        {
            get => _missions[index];
            set => _missions[index] = value;
        }
    }
}