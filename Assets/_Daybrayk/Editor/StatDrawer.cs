using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Daybrayk.rpg
{
	[CustomPropertyDrawer(typeof(Stat))]
	public class StatDrawer : PropertyDrawer
	{
        bool unfold = false;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if(unfold == true)
            {
                int n = 2;//number of fields in the property drawer
                int m = n * 2;//need to add extra height for the 2 pixel spaces that exist between fields

                return EditorGUIUtility.singleLineHeight*n + (n * 2);
            }
            else
            {
                return base.GetPropertyHeight(property, label);
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var labelRect = new Rect(position.x, position.y, position.width, 16);
            unfold = EditorGUI.Foldout(labelRect, unfold, label);
            if(unfold)
            {
                var baseValueRect = new Rect(position.x, position.y + 18, position.width, 16);
                EditorGUI.indentLevel++;
                EditorGUI.PropertyField(baseValueRect, property.FindPropertyRelative("_baseValue"));
                EditorGUI.indentLevel--;
            }
            EditorGUI.EndProperty();
    
        }
    }
}