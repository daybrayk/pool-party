using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class NetManagerUI : MonoBehaviour
{
    void OnGUI()
    {
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            StartButtons();
            GUILayout.EndArea();
        }

    }

    static void StartButtons()
    {
        if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
        if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
        if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
    }
}