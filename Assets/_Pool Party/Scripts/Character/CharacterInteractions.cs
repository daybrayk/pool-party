using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInteractions : MonoBehaviour
{
    CharacterRoot root;

    private void Awake()
    {
        root = GetComponent<CharacterRoot>();
    }

    private void OnTriggerEnter2D(Collider2D c)
    {
        if(c.CompareTag("Refill"))
        {
            root.weapon.inRefillZone = true;
        }
    }
    private void OnTriggerExit2D(Collider2D c)
    {
        if (c.CompareTag("Refill"))
        {
            root.weapon.inRefillZone = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D c)
    {
        
    }
}