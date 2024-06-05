using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Daybrayk;
public class DroppedFlag : NetworkBehaviour
{
    public ulong flagId { get; set; }
    [SerializeField]
    GameObject visrep;
    new Collider2D collider;

    private void Awake()
    {
        collider = GetComponent<Collider2D>();
        HideFlag();
    }

    public override void OnNetworkSpawn()
	{
		
	}

    public void ShowFlag()
    {
        visrep.SetActive(true);
        collider.enabled = true;
    }

    public void HideFlag()
    {
        visrep.SetActive(false);
        collider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D c)
    {
        if (!IsServer) return;
        
        if (c.CompareTag("Player"))
        {
			if (c.TryGetComponent(out CharacterRoot root))
            {
                var mode = GameModeBase.instance as CaptureFlagMode;
                if (root.owningPlayer.teamId.Value == flagId) 
                {
                    mode.FlagReturned(flagId);
                }
                else
                {
                    mode.FlagTaken(flagId, root.owningPlayer.clientId);
                    root.flag.TakeFlag(this);
                }
            }
        }
    }
}