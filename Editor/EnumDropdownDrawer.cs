using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Platinio.AdvancedDropdown
{
    [CustomPropertyDrawer(typeof(EnumDropdown))]
    public class EnumDropdownDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position,label,property);
            
            Rect labelRect = position;
            labelRect.width = position.width / 2.0f;
            
            Rect dropdownRect = position;
            dropdownRect.x += position.width / 2.0f;
            dropdownRect.width = position.width / 2.0f;
            
            EditorGUI.LabelField(labelRect, label);
            DrawEnumDropdown(dropdownRect, property);

            EditorGUI.EndProperty();
        }
        
        private void DrawEnumDropdown(Rect rect, SerializedProperty property)
        {
            if (!EditorGUI.DropdownButton(rect, new GUIContent(GetSelectedItemName(property)), FocusType.Passive)) return;

            
            var dropDownItems = GetDropdownItems(property);

            AdvancedDropdownEditorWindow.ShowDropdown(dropDownItems, delegate(string item)
            {
                UpdateDropdownValue(property, item);
            });
        }
        
        private void UpdateDropdownValue(SerializedProperty property, string item)
        {
            property.serializedObject.Update();
            property.enumValueIndex = GetEnumValueIndex(property, item);
            property.serializedObject.ApplyModifiedProperties();
        }

        private int GetEnumValueIndex(SerializedProperty property, string item)
        {
            for (int i = 0; i < property.enumDisplayNames.Length; i++)
            {
                if (item == property.enumDisplayNames[i]) return  i;
            }

            return -1;
        }

        private List<DropdownItem<string>> GetDropdownItems(SerializedProperty property)
        {
            List<DropdownItem<string>> dropDownItems = new();
            foreach (var enumName in property.enumDisplayNames)
            {
                bool isSelected = enumName == property.enumDisplayNames[property.enumValueIndex];
                dropDownItems.Add(new DropdownItem<string>(enumName, null,  isSelected, enumName));
            }
            
            return dropDownItems;
        }
        
        private string GetSelectedItemName(SerializedProperty property)
        {
            return property.enumDisplayNames[property.enumValueIndex];
        }
    }
}

