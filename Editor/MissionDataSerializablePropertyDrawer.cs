using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Utils.Editor;
using Utils.Editor.EditorGUIUtils;

namespace Missions.Editor
{
    [CustomPropertyDrawer(typeof(MissionDataSerializable))]
    public class MissionDataSerializablePropertyDrawer : PropertyDrawer
    {
        private static readonly GUIContent IsCompletedLabel = new ("Is Completed");

        private static readonly Dictionary<string, Func<SerializedProperty, VisualElement>> PropertyDrawerAdapters = new()
        {
            {MissionDataSerializable.IsClaimedPropertyName, GetIsClaimedPropertyField},
        };

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            // var isCompletedField = new IMGUIContainer(() =>
            // {
            //     var target = property.boxedValue as MissionDataSerializable;
            //     var isCompleted = target.Mission.IsCompleted;
            //     EditorGUI.BeginDisabledGroup(true);
            //     var rect = EditorGUILayout.GetControlRect();
            //     var propertyRect = EditorGUI.PrefixLabel(rect, IsCompletedLabel);
            //     EditorGUI.Toggle(propertyRect, isCompleted);
            //     EditorGUI.EndDisabledGroup();
            // });
            var target = property.boxedValue as MissionDataSerializable;
            var isCompletedField = new IsCompletedToggle("Is Completed", target);
            root.Add(isCompletedField);
            
            var subproperties = PropertiesUtils.GetSerializedProperties(property);
            GUIUtils.DrawSerializedProperties(root, subproperties, PropertyDrawerAdapters);

            return root;
        }

        private static VisualElement GetIsClaimedPropertyField(SerializedProperty property)
        {
            var propertyField = new PropertyField(property);
            propertyField.AddToClassList("unity-base-field__aligned");  // make widths aligned with other fields in inspector
            propertyField.Bind(property.serializedObject);  // doesn't work without binding manually in PropertyDrawers
            propertyField.SetEnabled(false);
            
            return propertyField;
        }
    }

    public class IsCompletedToggle : Toggle
    {
        private readonly MissionDataSerializable _mission;

        public IsCompletedToggle(string label, MissionDataSerializable mission) : base(label)
        {
            _mission = mission;
            SetEnabled(false);
            AddToClassList("unity-base-field__aligned");
            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        private void OnAttachToPanel(AttachToPanelEvent evt)
        {
            value = _mission.Mission.IsCompleted;
            _mission.Mission.OnCompleted += OnCompleted;
        }

        private void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            _mission.Mission.OnCompleted -= OnCompleted;
        }

        private void OnCompleted()
        {
            value = _mission.Mission.IsCompleted;
        }
    }
}