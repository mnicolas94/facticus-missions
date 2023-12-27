using UnityEngine;

namespace Missions
{
    [CreateAssetMenu(fileName = "Mission", menuName = "Facticus/Missions/Mission", order = 0)]
    public class MissionScriptableObject : ScriptableObject
    {
        [SerializeReference, SubclassSelector] private IMission _mission;

        public IMission Mission => _mission;
    }
}