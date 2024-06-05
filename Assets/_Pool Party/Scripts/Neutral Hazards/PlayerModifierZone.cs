using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerModifierZone : NetworkBehaviour
{
	protected List<CharacterRoot> players;

    protected void Awake()
    {
        players = new List<CharacterRoot>();
    }

    protected virtual void Execute() { }

    protected virtual void OnTriggerEnter2D(Collider2D c)
    {
        if (!IsServer) return;
        if (c.TryGetComponent(out CharacterRoot root) && !players.Contains(root))
        {
            players.Add(root);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D c)
    {
        if (!IsServer) return;

        if (c.TryGetComponent(out CharacterRoot root) && players.Contains(root))
        {
            players.Remove(root);
        }
    }
}