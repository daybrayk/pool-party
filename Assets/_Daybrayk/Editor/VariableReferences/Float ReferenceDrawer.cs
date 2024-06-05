#if UNITY_EDITOR
using UnityEditor;

namespace Daybrayk.Properties
{
    [CustomPropertyDrawer(typeof(FloatReference))]
    public class FloatReferenceDrawer : VariableReferenceDrawerBase { }
}
#endif
