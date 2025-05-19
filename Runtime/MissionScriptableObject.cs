using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace Missions
{
    [CreateAssetMenu(fileName = "Mission", menuName = "Facticus/Missions/Mission", order = 0)]
    public class MissionScriptableObject : ScriptableObject
    {
        private const string DescriptionSelfKey = "self";
        
        [SerializeReference, SubclassSelector] private IMission _mission;

        public IMission Mission => _mission;

#if UNITY_EDITOR
        
        private void OnValidate()
        {
            var description = _mission.GetDescription();
            if (!description.ContainsKey(DescriptionSelfKey))
            {
                description.Add(DescriptionSelfKey, new ObjectVariable(){ Value = this});
                EditorUtility.SetDirty(this);
            }
        }
        
#endif
    }
}