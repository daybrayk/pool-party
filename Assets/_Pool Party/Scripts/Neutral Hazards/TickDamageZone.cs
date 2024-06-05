using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickDamageZone : PlayerModifierZone
{
    [SerializeField]
    protected float tickRate;
    [SerializeField]
    protected int tickDamage;

    protected float timer;

    protected void Update()
    {
        if (timer > tickRate)
        {
            timer = 0;
            for (int i = 0; i < players.Count; i++)
            {
                players[i].combat.ApplyDamage(tickDamage);
            }
        }

        timer += Time.deltaTime;
    }
}
