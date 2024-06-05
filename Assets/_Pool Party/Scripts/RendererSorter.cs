using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Daybrayk;
public class RendererSorter : MonoBehaviour
{
	[SerializeField]
	new Renderer renderer;
    [SerializeField]
    SortingGroup group;
    bool useGroup { get; set; } = false;
    [SerializeField]
    int sortingOrderBase = 5000;
    [SerializeField]
    float offset;

    private void Start()
    {
        if (group != null) useGroup = true;
    }

    private void LateUpdate()
    {
        if (useGroup)
        {
            group.sortingOrder = (int)(sortingOrderBase - transform.position.y + offset);
        }
        else
        {
            renderer.sortingOrder = (int)(sortingOrderBase - transform.position.y + offset);
        }
    }

    private void OnValidate()
    {
        if (group != null) useGroup = true;

        if (useGroup)
        {
            group.sortingOrder = (int)(sortingOrderBase - transform.position.y + offset);
        }
        else
        {
            renderer.sortingOrder = (int)(sortingOrderBase - transform.position.y + offset);
        }
    }
}