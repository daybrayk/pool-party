using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using UnityEngine.SceneManagement;

public class CharacterCameraController : NetworkBehaviour
{
    [SerializeField]
    GameObject cameraFollow;
    public CinemachineVirtualCamera vCam { get; private set; }
    public Camera mainCamera { get; private set; }

    CharacterRoot root;

    private void Start()
    {
        root = GetComponent<CharacterRoot>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner && IsClient)
        {
            vCam = FindObjectOfType<CinemachineVirtualCamera>();
            if (vCam == null)
            {
                Debug.Log("No vCam was found in scene " + SceneManager.GetActiveScene().name);
                return;
            }

            vCam.Follow = cameraFollow.transform;
            mainCamera = Camera.main;

            //Likely temporary while we are using Server Authoritative movement
            if (IsHost) mainCamera.GetComponent<CinemachineBrain>().m_UpdateMethod = CinemachineBrain.UpdateMethod.FixedUpdate;
            else if(IsClient)
            {
                mainCamera.GetComponent<CinemachineBrain>().m_UpdateMethod = CinemachineBrain.UpdateMethod.SmartUpdate;
            }
        }
    }
}