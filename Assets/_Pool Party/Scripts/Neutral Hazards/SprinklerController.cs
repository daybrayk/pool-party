using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Daybrayk;

public class SprinklerController : MonoBehaviour
{
    [System.Serializable]
    struct StreamRenderer
    {
        public LineRenderer renderer;
        public Transform middleBezierPoint;
        public float thirdBezierOffset;
    }

    [Header("Water Rendering")]
    [SerializeField]
    int lineRenderPositionCount = 20;
    [SerializeField]
    GameObject waterDropVFX;
    [SerializeField]
    StreamRenderer[] streams;

    [Header("Movement")]
    [SerializeField]
    GameObject movingTrigger;
    [SerializeField]
    float moveSpeed = 0.5f;
    [SerializeField]
    float moveDistance = 2f;

    [Header("Debug")]
    [SerializeField]
    [ReadOnly]
    Vector3[] midBezierPoints;
    [SerializeField]
    [ReadOnly]
    Vector3 triggerPosition;

    float zeroPosition;
    float accumulator;
    private void Start()
    {
        triggerPosition = transform.position;
        midBezierPoints = new Vector3[streams.Length];

        for (int i = 0; i < midBezierPoints.Length; i++)
        {
            midBezierPoints[i] = streams[i].middleBezierPoint.position;
            streams[i].renderer.positionCount = lineRenderPositionCount;
        }
        
        zeroPosition = transform.position.x;
    }

    private void Update()
    {
        triggerPosition.x = zeroPosition + Mathf.Sin(accumulator) * moveDistance;

        for (int i = 0; i < streams.Length; i++)
        {
            midBezierPoints[i].x = zeroPosition + Mathf.Sin(accumulator) * (moveDistance / 1.5f);
            streams[i].middleBezierPoint.position = midBezierPoints[i];
        }

        movingTrigger.transform.position = triggerPosition;

        accumulator += Time.deltaTime * moveSpeed;

        UpdateLineRenderers();

        if (Time.frameCount % 15 == 0) SpawnFX();
    }

    void UpdateLineRenderers()
    {
        for (int i = 0; i < lineRenderPositionCount; i++)
        {
            for (int j = 0; j < streams.Length; j++)
            {
                streams[j].renderer.SetPosition(i, MathUtil.QuadraticBezierPoint((float)i / (float)(lineRenderPositionCount - 1), streams[j].renderer.transform.position, streams[j].middleBezierPoint.position, triggerPosition + (Vector3.up * streams[j].thirdBezierOffset)));
            }
        }
    }

    void SpawnFX()
    {
        for (int i = 0; i < streams.Length; i++)
        {
            Destroy(Instantiate(waterDropVFX, streams[i].renderer.GetPosition(lineRenderPositionCount - 1), Quaternion.identity), 0.1f);
        }
    }
}
