using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Daybrayk;
public class LinkedProjectile : ProjectileBase
{
	[SerializeField]
	LineRenderer _lineRenderer;
	public LineRenderer lineRenderer => _lineRenderer;
    [SerializeField]
    [ReadOnly]
    LinkedProjectile _previousProjectile;

    public LinkedProjectile previousProjectile
    {
        get { return _previousProjectile; }
        set
        {
            _previousProjectile = value;
            if (value == null) lineRenderer.positionCount = 1;
            else
            {
                lineRenderer.positionCount = 2;
            }
        }
    }

    public LinkedProjectile nextProjectile { get; set; }

    new void Update()
    {
        base.Update();

        UpdateLineRenderer();
    }

    void UpdateLineRenderer()
    {
        if (lineRenderer.enabled)
        {
            lineRenderer.SetPosition(0, transform.position);
            if (previousProjectile != null)
            {
                if (Time.frameCount % 4 == 0)
                {
                    if (Vector2.Distance(previousProjectile.transform.position, transform.position) > 3)
                    {
                        LinkedProjectile p = Instantiate(gameObject, transform.position + (0.5f * (previousProjectile.transform.position - transform.position)), Quaternion.Lerp(transform.rotation, previousProjectile.transform.rotation, 0.5f)).GetComponent<LinkedProjectile>();

                        p.Init(Vector2.Lerp(previousProjectile.velocity, velocity, 0.5f), owner);
                        p.lifeTime = lifeTime;
                        p.previousProjectile = previousProjectile;
                        p.nextProjectile = this;
                        previousProjectile.nextProjectile = p;
                        previousProjectile = p;
                    }
                }

                if (lineRenderer.positionCount >= 2)
                {
                    lineRenderer.SetPosition(1, previousProjectile.transform.position);
                }
            }
        }
    }

    protected override void BeginDestroy()
    {
        lineRenderer.enabled = false;
        if (nextProjectile != null) nextProjectile.previousProjectile = null;
        if (previousProjectile != null)
        {
            previousProjectile.nextProjectile = null;
            lineRenderer.positionCount = 1;
        }
        base.BeginDestroy();
    }
}