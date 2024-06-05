#if UNITY_EDITOR
using UnityEditor;

namespace Daybrayk.Properties
{
    [CustomPropertyDrawer(typeof(Vector3Reference))]
    public class Vector3ReferenceDrawer : VariableReferenceDrawerBase { }
}
#endif