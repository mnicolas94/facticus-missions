using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Utils.Editor;
using Utils.Editor.EditorGUIUtils;

namespace Missions.Editor
{
    [CustomPropertyDrawer(typeof(MissionDataSerializable))]
    public class MissionDataSerializablePropertyDrawer : PropertyDrawer
    {
        private static readonly Dictionary<string, Func<SerializedProperty, VisualElement>> PropertyDrawerAdapters = new()
        {
            {MissionDataSerializable.IsClaimedPropertyName, GetIsClaimedPropertyField},
        };

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            var subproperties = PropertiesUtils.GetSerializedProperties(property).ToList();
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
}