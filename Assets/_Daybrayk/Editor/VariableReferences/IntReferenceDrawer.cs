#if UNITY_EDITOR
using UnityEditor;

namespace Daybrayk.Properties
{
    [CustomPropertyDrawer(typeof(IntReference))]
    public class IntReferenceDrawer : VariableReferenceDrawerBase { }
}
#endif
