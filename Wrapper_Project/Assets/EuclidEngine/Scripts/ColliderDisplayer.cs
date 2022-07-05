using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(LineRenderer))]
public class ColliderDisplayer : MonoBehaviour
{
    BoxCollider m_boxCollider;
    LineRenderer m_lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        m_boxCollider = GetComponent<BoxCollider>();
        m_lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        var size = m_boxCollider.size;
        var baseTransform = new Vector3(-size.x * 0.5f, -size.y * 0.5f, -size.z * 0.5f);

        Vector3[] pts = {
            baseTransform + Vector3.zero,
            baseTransform + new Vector3(size.x, 0, 0),
            baseTransform + new Vector3(size.x, size.y, 0),
            baseTransform + new Vector3(0, size.y, 0),
            baseTransform + new Vector3(0, 0, size.z),
            baseTransform + new Vector3(size.x, 0, size.z),
            baseTransform + new Vector3(size.x, size.y, size.z),
            baseTransform + new Vector3(0, size.y, size.z)
        };

        Vector3[] ptsSet =
        {
            pts[0], pts[1], pts[2], pts[3], pts[0],
            pts[4], pts[5], pts[6], pts[7], pts[4],
            pts[0], pts[1], pts[5],
            pts[6], pts[2], pts[3], pts[7]
        };

        m_lineRenderer.positionCount = ptsSet.Length;
        m_lineRenderer.SetPositions(ptsSet);
    }
}
