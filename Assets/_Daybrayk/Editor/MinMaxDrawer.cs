#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Daybrayk
{
    [CustomPropertyDrawer(typeof(MinMaxAttribute))]
    public class MinMaxDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            MinMaxAttribute @Attribute = (MinMaxAttribute)attribute;

            label.tooltip = Attribute.min.ToString("F2") + " to " + Attribute.max.ToString("F2");

            Rect controlRect = EditorGUI.PrefixLabel(position, label);

            Rect[] splittedRect = SplitRect(controlRect, 3);

            if (property.propertyType == SerializedPropertyType.Vector2)
            {

                EditorGUI.BeginChangeCheck();

                Vector2 vector = property.vector2Value;
                float minVal = vector.x;
                float maxVal = vector.y;

                //F2 limits the float to two decimal places (0.00).
                minVal = EditorGUI.FloatField(splittedRect[0], float.Parse(minVal.ToString("F2")));
                maxVal = EditorGUI.FloatField(splittedRect[2], float.Parse(maxVal.ToString("F2")));

                EditorGUI.MinMaxSlider(splittedRect[1], ref minVal, ref maxVal, Attribute.min, Attribute.max);

                if (minVal < Attribute.min)
                {
                    Attribute.min = minVal;
                }

                if (maxVal > Attribute.max)
                {
                    Attribute.max = maxVal;
                }

                vector = new Vector2(minVal, minVal > maxVal ? minVal : maxVal);

                if (EditorGUI.EndChangeCheck())
                {
                    property.vector2Value = vector;
                }

            }
            else if (property.propertyType == SerializedPropertyType.Vector2Int)
            {

                EditorGUI.BeginChangeCheck();

                Vector2Int vector = property.vector2IntValue;
                float minVal = vector.x;
                float maxVal = vector.y;

                minVal = EditorGUI.FloatField(splittedRect[0], minVal);
                maxVal = EditorGUI.FloatField(splittedRect[2], maxVal);

                EditorGUI.MinMaxSlider(splittedRect[1], ref minVal, ref maxVal,
                Attribute.min, Attribute.max);

                if (minVal < Attribute.min)
                {
                    Attribute.min = maxVal;
                }

                if (minVal > Attribute.max)
                {
                    Attribute.max = maxVal;
                }

                vector = new Vector2Int(Mathf.FloorToInt(minVal), Mathf.FloorToInt(minVal > maxVal ? maxVal : maxVal));

                if (EditorGUI.EndChangeCheck())
                {
                    property.vector2IntValue = vector;
                }

            }

        }

        Rect[] SplitRect(Rect rectToSplit, int n)
        {


            Rect[] rects = new Rect[n];

            for (int i = 0; i < n; i++)
            {

                rects[i] = new Rect(rectToSplit.position.x + (i * rectToSplit.width / n), rectToSplit.position.y, rectToSplit.width / n, rectToSplit.height);

            }

            int padding = (int)rects[0].width - 40;
            int space = 5;

            rects[0].width -= padding + space;
            rects[2].width -= padding + space;

            rects[1].x -= padding;
            rects[1].width += padding * 2;

            rects[2].x += padding + space;


            return rects;

        }
    }
        
}
#endif
