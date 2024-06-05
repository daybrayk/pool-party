using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasRenderSorter : MonoBehaviour
{
    [SerializeField]
    Canvas canvas;
    [SerializeField]
    int sortingOrderBase = 5000;
    [SerializeField]
    float offset;

    private void LateUpdate()
    {
        canvas.sortingOrder = (int)(sortingOrderBase - transform.position.y + offset);
    }

    private void OnValidate()
    {
        canvas.sortingOrder = (int)(sortingOrderBase - transform.position.y + offset);
    }
}