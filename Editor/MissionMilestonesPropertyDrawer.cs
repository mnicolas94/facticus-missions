using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Utils.Editor;
using Utils.Editor.EditorGUIUtils;

namespace Missions.Editor
{
    [CustomPropertyDrawer(typeof(MissionMilestones))]
    public class MissionMilestonesPropertyDrawer : PropertyDrawer
    {
        private MissionMilestones _milestones;
        private PropertyField _intervalPropertyField;
        private PropertyField _intervalMilestonePropertyField;
        private PropertyField _milestonesPropertyField;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            
            _milestones = property.boxedValue as MissionMilestones;
            
            Dictionary<string, Func<SerializedProperty, VisualElement>> propertyDrawerAdapters = new()
            {
                {MissionMilestones.MilestonesModePropertyName, DrawMilestonesModeProperty},
                {MissionMilestones.IntervalPropertyName, DrawIntervalProperty},
                {MissionMilestones.IntervalMilestonePropertyName, DrawIntervalMilestoneProperty},
                {MissionMilestones.MilestonesPropertyName, DrawMilestonesProperty},
            };
            
            var subproperties = PropertiesUtils.GetSerializedProperties(property);
            GUIUtils.DrawSerializedProperties(root, subproperties, propertyDrawerAdapters);

            return root;
        }

        private VisualElement DrawMilestonesModeProperty(SerializedProperty property)
        {
            var propertyField = new PropertyField(property);
            propertyField.RegisterValueChangeCallback(UpdateVisibility);
            return propertyField;
        }

        private void UpdateVisibility(SerializedPropertyChangeEvent evt)
        {
            var mode = (MissionMilestones.MilestonesMode) evt.changedProperty.enumValueIndex;
            var isInterval = mode == MissionMilestones.MilestonesMode.Interval;
            _intervalPropertyField.style.display = isInterval ? DisplayStyle.Flex : DisplayStyle.None;
            _intervalMilestonePropertyField.style.display = isInterval ? DisplayStyle.Flex : DisplayStyle.None;
            _milestonesPropertyField.style.display = isInterval ? DisplayStyle.None : DisplayStyle.Flex;
        }

        private VisualElement DrawIntervalProperty(SerializedProperty property)
        {
            _intervalPropertyField = new PropertyField(property);
            var visible = _milestones.MilestonesInputMode == MissionMilestones.MilestonesMode.Interval;
            _intervalPropertyField.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
            return _intervalPropertyField;
        }

        private VisualElement DrawIntervalMilestoneProperty(SerializedProperty property)
        {
            _intervalMilestonePropertyField = new PropertyField(property);
            var visible = _milestones.MilestonesInputMode == MissionMilestones.MilestonesMode.Interval;
            _intervalMilestonePropertyField.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
            return _intervalMilestonePropertyField;
        }

        private VisualElement DrawMilestonesProperty(SerializedProperty property)
        {
            _milestonesPropertyField = new PropertyField(property);
            var visible = _milestones.MilestonesInputMode == MissionMilestones.MilestonesMode.Manual;
            _milestonesPropertyField.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
            return _milestonesPropertyField;
        }
    }
}