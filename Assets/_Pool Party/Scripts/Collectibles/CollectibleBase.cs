using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public abstract class CollectibleBase : NetworkBehaviour
{
	public override void OnNetworkSpawn()
	{
		
	}

    protected virtual void OnTriggerEnter2D(Collider2D c)
    {
        if(c.TryGetComponent(out CharacterRoot root)) Collect(root);
    }

    protected abstract void Collect(CharacterRoot root);
}