#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisposableEditorMethods { }

public struct Horizontal : System.IDisposable
{
    public Horizontal(params GUILayoutOption[] options)
    {
        GUILayout.BeginHorizontal(options);
    }

    public void Dispose()
    {
        GUILayout.EndHorizontal();
    }
}

public struct Vertical : System.IDisposable
{
    public Vertical(params GUILayoutOption[] options)
    {
        GUILayout.BeginVertical(options);
    }

    public void Dispose()
    {
        GUILayout.EndVertical();
    }
}
#endif
