using Missions.CreationStrategies;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Utils.Editor;

namespace Missions.Editor
{
    [CustomEditor(typeof(MissionsCyclicCreator))]
    public class MissionsCyclicCreatorEditor : UnityEditor.Editor
    {
        private MissionsCyclicCreator _creator;

        public override VisualElement CreateInspectorGUI()
        {
            _creator = target as MissionsCyclicCreator;

            var properties = PropertiesUtils.GetSerializedProperties(serializedObject);
            var root = new VisualElement();
            foreach (var serializedProperty in properties)
            {
                root.Add(new PropertyField(serializedProperty));
            }

            var refreshMissionsButton = new Button(RefreshMissions);
            refreshMissionsButton.text = "Refresh Missions";
            refreshMissionsButton.tooltip = "Will clear, ensure max and start missions in that order";
            root.Add(refreshMissionsButton);

            return root;
        }

        private void RefreshMissions()
        {
            _creator.Refresh();
        }
    }
}
