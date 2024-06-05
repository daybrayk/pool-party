#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Daybrayk.Properties
{
    public class VariableReferenceDrawerBase : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            var useReferenceLabelRect = new Rect(position.x, position.y, 90, position.height);
            var useReferenceRect = new Rect(position.x + 90, position.y, 30, position.height);
        
            var valueRect = new Rect(position.x + 110, position.y, position.width, position.height);
            var referenceRect = new Rect(position.x + 110, position.y, 100, position.height);

            EditorGUI.LabelField(useReferenceLabelRect, new GUIContent("Use Reference"));
            EditorGUI.PropertyField(useReferenceRect, property.FindPropertyRelative("useReference"), GUIContent.none);

            if(property.FindPropertyRelative("useReference").boolValue == true)
            {
            
                EditorGUI.PropertyField(referenceRect, property.FindPropertyRelative("reference"), GUIContent.none);
            }
            else
            {
                EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("value"), GUIContent.none);
            }
            EditorGUI.EndProperty();
            EditorGUI.indentLevel = indent;
        }
    }
}
#endif
