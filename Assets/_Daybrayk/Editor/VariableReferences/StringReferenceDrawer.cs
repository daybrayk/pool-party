#if UNITY_EDITOR
using UnityEditor;

namespace Daybrayk.Properties
{
    [CustomPropertyDrawer(typeof(StringReference))]
    public class StringReferenceDrawer : VariableReferenceDrawerBase { }
}
#endif
