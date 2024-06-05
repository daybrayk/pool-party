#if UNITY_EDITOR
using UnityEditor;

namespace Daybrayk.Properties
{
    [CustomPropertyDrawer(typeof(BoolReference))]
    public class BoolReferenceDrawer : VariableReferenceDrawerBase { }
}
#endif
