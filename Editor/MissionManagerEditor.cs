
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Utils.Editor;

namespace Missions.Editor
{
    [CustomEditor(typeof(MissionsManager))]
    public class MissionManagerEditor : UnityEditor.Editor
    {
        private MissionsManager _missionManager;

        public override VisualElement CreateInspectorGUI()
        {
            _missionManager = target as MissionsManager;

            var properties = PropertiesUtils.GetSerializedProperties(serializedObject);
            var root = new VisualElement();
            foreach (var serializedProperty in properties)
            {
                root.Add(new PropertyField(serializedProperty));
            }

            var startMissionsButton = new Button(StartMissions);
            startMissionsButton.text = "Start Missions";
            root.Add(startMissionsButton);
            
            var clearMissionsButton = new Button(ClearMissions);
            clearMissionsButton.text = "Clear Missions";
            root.Add(clearMissionsButton);

            return root;
        }

        private void StartMissions()
        {
            _missionManager.StartMissions();
        }

        private void ClearMissions()
        {
            _missionManager.ClearMissions();
        }
    }
}
