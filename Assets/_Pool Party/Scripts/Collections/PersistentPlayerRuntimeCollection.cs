using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Network/Persistent Player Collection")]
public class PersistentPlayerRuntimeCollection : RuntimeCollection<PersistentPlayer>
{
    public bool TryGetPlayer(ulong clientId, out PersistentPlayer persistentPlayer)
    {
        for (int i = items.Count-1; i >= 0; i--)
        {
            var player = items[i];
            if (player == null)
            {
                items.RemoveAt(i);
                continue;
            }
            
            if (player.OwnerClientId == clientId)
            {
                persistentPlayer = items[i];
                return true;
            }
        }

        persistentPlayer = null;
        return false;
    }
}