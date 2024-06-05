using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class FadeAlphaOnEnter : MonoBehaviour
{
    [Header("Enter")]
    [SerializeField]
    GameObject[] turnOffOnEnter;
    [SerializeField]
    GameObject[] turnOnOnEnter;

    [Header("Exit")]
    [SerializeField]
    GameObject[] turnOffOnExit;
    [SerializeField]
    GameObject[] turnOnOnExit;

    [SerializeField]
	new Collider2D collider;

    private void Start()
    {
        if (collider == null && !TryGetComponent(out collider)) Debug.LogError("There is no collider component on " + gameObject.name, this);
        else
        {
            collider.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D c)
    {
        CharacterRoot root;
        if (c.TryGetComponent(out root))
        {
            if (root.IsLocalPlayer)
            {

                for (int i = 0; i < turnOffOnEnter.Length; i++)
                {
                    turnOffOnEnter[i].SetActive(false);
                }
                for (int i = 0; i < turnOnOnEnter.Length; i++)
                {
                    turnOnOnEnter[i].SetActive(true);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D c)
    {
        CharacterRoot root;
        if (c.TryGetComponent(out root))
        {
            if (root.IsLocalPlayer)
            {
                for (int i = 0; i < turnOnOnExit.Length; i++)
                {
                    turnOnOnExit[i].SetActive(true);
                }

                for (int i = 0; i < turnOffOnExit.Length; i++)
                {
                    turnOffOnExit[i].SetActive(false);
                }
            }
        }
    }
}