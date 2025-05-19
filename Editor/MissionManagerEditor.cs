
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

            var ensureMaxMissionsButton = new Button(EnsureMaxMissions);
            ensureMaxMissionsButton.text = "Ensure Max Missions";
            root.Add(ensureMaxMissionsButton);
            
            var refreshMissionsButton = new Button(RefreshMissions);
            refreshMissionsButton.text = "Refresh Missions";
            refreshMissionsButton.tooltip = "Will clear, ensure max and start missions in that order";
            root.Add(refreshMissionsButton);

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

        private void EnsureMaxMissions()
        {
            _missionManager.EnsureMaxMission();
        }

        private void RefreshMissions()
        {
            ClearMissions();
            EnsureMaxMissions();
            StartMissions();
        }
    }
}
