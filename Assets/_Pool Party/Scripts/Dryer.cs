using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dryer : MonoBehaviour
{
    [SerializeField]
    float interval;
    [SerializeField]
    int amount;

    List<CharacterRoot> players;

    float accumulator = 0;

    private void Awake()
    {
        players = new List<CharacterRoot>();
    }

    private void Update()
    {
        if(accumulator > interval)
        {
            int playerCount = players.Count - 1;
            for (int i = playerCount; i >= 0; i--)
            {
                if (players[i] == null || !players[i].gameObject.activeSelf) players.RemoveAt(i);
                players[i].combat.RemoveDamage(amount);
            }
            accumulator = 0;
        }
        else
        {
            accumulator += Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D c)
    {
        CharacterRoot root;
        if (c.TryGetComponent(out root))
        {
            if (!players.Contains(root)) players.Add(root);
        }
    }

    private void OnTriggerExit2D(Collider2D c)
    {
        CharacterRoot root;
        if(c.TryGetComponent(out root))
        {
            players.Remove(root);
        }
    }

    private void OnValidate()
    {
        interval = Mathf.Max(interval, 0);
        amount = Mathf.Max(amount, 1);
    }
}