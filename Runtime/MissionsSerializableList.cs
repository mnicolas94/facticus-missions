using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Missions
{
    [CreateAssetMenu(fileName = "MissionsList", menuName = "Facticus/Missions/Missions list", order = 0)]
    public class MissionsSerializableList : ScriptableObject, IList<IMission> 
    {
        [SerializeReference] private List<IMission> _missions = new();

        public Action<IMission> Added;
        public Action<IMission> Removed;
        
        public IEnumerator<IMission> GetEnumerator()
        {
            _missions ??= new List<IMission>();
            return _missions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_missions).GetEnumerator();
        }

        public void Add(IMission item)
        {
            _missions.Add(item);
            Added?.Invoke(item);
        }

        public void Clear()
        {
            var temp = new List<IMission>(_missions);
            _missions.Clear();
            foreach (var mission in temp)
            {
                Removed?.Invoke(mission);
            }
        }

        public bool Contains(IMission item)
        {
            return _missions.Contains(item);
        }

        public void CopyTo(IMission[] array, int arrayIndex)
        {
            _missions.CopyTo(array, arrayIndex);
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

        public int Count => _missions.Count;

        public bool IsReadOnly => ((ICollection<IMission>)_missions).IsReadOnly;

        public int IndexOf(IMission item)
        {
            return _missions.IndexOf(item);
        }

        public void Insert(int index, IMission item)
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

        public IMission this[int index]
        {
            get => _missions[index];
            set => _missions[index] = value;
        }
    }
}